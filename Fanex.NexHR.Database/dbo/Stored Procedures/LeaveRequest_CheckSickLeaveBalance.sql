
CREATE PROCEDURE [dbo].[LeaveRequest_CheckSickLeaveBalance] @EmpId INT
	,@LeaveDays DECIMAL(18, 2) = NULL
	,@Year INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2021-08-25
Task: check sick leave balance
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT @LeaveDays = ISNULL(@LeaveDays, 0)

	DECLARE @EnableSickLeaveBalance BIT = 0

	SELECT @EnableSickLeaveBalance = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'EnableSickLeaveBalance'

	IF @EnableSickLeaveBalance = 1
		AND @LeaveDays > 0
		AND @LeaveDays > dbo.fnc_GetSickLeaveBalance(@EmpId, @Year, NULL)
	BEGIN
		SET @ErrCode = 109
		SET @ErrMsg = 'You have not enough sick leave days to request.'
	END
END