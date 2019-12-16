
CREATE PROCEDURE [dbo].[Postnatal_Update] @PostnatalId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Minute SMALLINT
	,@ModifiedBy INT
	,@Remark NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Update Postnatal.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeePostnatal WITH (NOLOCK)
			WHERE PostnatalId <> @PostnatalId
				AND EmpId = @EmpId
				AND FromDate = @FromDate
				AND ToDate = @ToDate
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee from' + CONVERT(VARCHAR(10), @FromDate, 103) + ' and ' + CONVERT(VARCHAR(10), @ToDate, 103) + ' has already exists'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeePostnatal WITH (NOLOCK)
			WHERE PostnatalId = @PostnatalId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee does not exists'

		RETURN
	END

	UPDATE dbo.EmployeePostnatal WITH (ROWLOCK, UPDLOCK)
	SET EmpId = @EmpId
		,FromDate = @FromDate
		,ToDate = @ToDate
		,[Minute] = @Minute
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
		,Remark = @Remark
	WHERE PostnatalId = @PostnatalId
END
