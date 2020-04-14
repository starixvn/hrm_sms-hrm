
CREATE PROCEDURE [dbo].[Paternity_Update] @PaternityId INT
	,@EmpId INT
	,@ChildBornDate DATE
	,@LeaveDay INT
	,@ModifiedBy INT
	,@Remark NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Update Paternity.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @OldBalanceDay INT

	SELECT @OldBalanceDay = LeaveDay
	FROM dbo.EmployeePaternity WITH (NOLOCK)
	WHERE PaternityId = @PaternityId

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeePaternity WITH (NOLOCK)
			WHERE PaternityId <> @PaternityId
				AND EmpId = @EmpId
				AND ChildBornDate = @ChildBornDate
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee has children born on' + CONVERT(VARCHAR(10), @ChildBornDate, 103) + ' has already exists'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeePaternity WITH (NOLOCK)
			WHERE PaternityId = @PaternityId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee does not exists'

		RETURN
	END

	UPDATE dbo.EmployeePaternity WITH (ROWLOCK, UPDLOCK)
	SET EmpId = @EmpId
		,@ChildBornDate = @ChildBornDate
		,LeaveDay = @LeaveDay
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
		,Remark = @Remark
		,BalanceDay = BalanceDay + (@LeaveDay - @OldBalanceDay)
	WHERE PaternityId = @PaternityId
END