
CREATE PROCEDURE [dbo].[LeaveRequest_CheckHospitalizationLeaveBalance] @EmpId INT
	,@LeaveDays DECIMAL(18, 2) = NULL
	,@Year INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2021-08-25
Task: check hospitalization leave balance
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT @LeaveDays = ISNULL(@LeaveDays, 0)

	DECLARE @EnableHospitalizationLeaveBalance BIT = 0

	SELECT @EnableHospitalizationLeaveBalance = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'EnableHospitalizationLeaveBalance'

	IF @EnableHospitalizationLeaveBalance = 1
		AND @LeaveDays > 0
		AND @LeaveDays > dbo.fnc_GetHospitalizationLeaveBalance(@EmpId, @Year, NULL)
	BEGIN
		SET @ErrCode = 109
		SET @ErrMsg = 'You have not enough hospitalization leave days to request.'
	END
END