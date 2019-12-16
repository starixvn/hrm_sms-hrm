
CREATE PROCEDURE [dbo].[Postnatal_Insert] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Minute SMALLINT
	,@CreatedBy INT
	,@Remark NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Insert Postnatal.
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
			WHERE EmpId = @EmpId
				AND FromDate = @FromDate
				AND ToDate = @ToDate
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee from' + CONVERT(VARCHAR(10), @FromDate, 103) + ' and ' + CONVERT(VARCHAR(10), @ToDate, 103) + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.EmployeePostnatal (
		EmpId
		,FromDate
		,ToDate
		,[Minute]
		,CreatedBy
		,CreatedDate
		,Remark
		)
	VALUES (
		@EmpId
		,@FromDate
		,@ToDate
		,@Minute
		,@CreatedBy
		,GETDATE()
		,@Remark
		)
END
