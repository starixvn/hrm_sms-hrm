
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/19/2016 5:22:11 PM ******/
CREATE PROCEDURE [dbo].[Postnatal_Update] @PostnatalId INT
	,@EmpId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@ModifiedBy INT
	,@Remark NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Update Postnatal.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeePostnatal WITH (NOLOCK)
				WHERE PostnatalId <> @PostnatalId
					AND EmpId = @EmpId
					AND FromDate = @FromDate
					AND ToDate = @ToDate
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee from' + CONVERT(VARCHAR(10), @FromDate, 103) + ' and ' + CONVERT(VARCHAR(10), @ToDate, 103) + ' has already exists'

		RETURN
	END

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeePostnatal WITH (NOLOCK)
				WHERE PostnatalId = @PostnatalId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee does not exists'

		RETURN
	END

	UPDATE dbo.EmployeePostnatal
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET EmpId = @EmpId
		,FromDate = @FromDate
		,ToDate = @ToDate
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
		,Remark = @Remark
	WHERE PostnatalId = @PostnatalId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update employee postnatal failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END