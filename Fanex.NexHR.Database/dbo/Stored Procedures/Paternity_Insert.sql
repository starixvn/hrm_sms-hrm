
CREATE PROCEDURE [dbo].[Paternity_Insert] @EmpId INT
	,@ChildBornDate DATE
	,@LeaveDay INT
	,@CreatedBy INT
	,@Remark NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Insert Paternity.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeePaternity WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ChildBornDate = @ChildBornDate
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee has children born on' + CONVERT(VARCHAR(10), @ChildBornDate, 103) + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.EmployeePaternity (
		EmpId
		,ChildBornDate
		,LeaveDay
		,CreatedBy
		,CreatedDate
		,Remark
		)
	VALUES (
		@EmpId
		,@ChildBornDate
		,@LeaveDay
		,@CreatedBy
		,GETDATE()
		,@Remark
		)
END