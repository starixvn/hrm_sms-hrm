
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/19/2016 5:22:11 PM ******/
CREATE PROCEDURE [dbo].[Paternity_Update] @PaternityId INT
	,@EmpId INT
	,@ChildBornDate DATETIME
	,@LeaveDay INT
	,@ModifiedBy INT
	,@Remark NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Update Paternity.
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
				FROM dbo.EmployeePaternity WITH (NOLOCK)
				WHERE PaternityId <> @PaternityId
					AND EmpId = @EmpId
					AND ChildBornDate = @ChildBornDate
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee has children born on' + CONVERT(varchar(10), @ChildBornDate, 103) + ' has already exists'

		RETURN
	END

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeePaternity WITH (NOLOCK)
				WHERE PaternityId = @PaternityId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee does not exists'

		RETURN
	END

	UPDATE dbo.EmployeePaternity
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET EmpId = @EmpId
		,@ChildBornDate = @ChildBornDate
		,LeaveDay = @LeaveDay
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
		,Remark = @Remark
	WHERE PaternityId = @PaternityId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update employee paternity failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END