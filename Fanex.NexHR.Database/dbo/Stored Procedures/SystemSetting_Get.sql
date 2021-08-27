
CREATE PROCEDURE [dbo].[SystemSetting_Get]
AS
/*
Created: Tim.Trinh@2021-08-24
Task: get system setting
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EnableHospitalizationLeaveBalance BIT = 0
		,@EnableSickLeaveBalance BIT = 0

	SELECT @EnableHospitalizationLeaveBalance = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'EnableHospitalizationLeaveBalance'

	SELECT @EnableSickLeaveBalance = CAST(Value AS BIT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'EnableSickLeaveBalance'

	SELECT @EnableHospitalizationLeaveBalance AS EnableHospitalizationLeaveBalance
		,@EnableSickLeaveBalance AS EnableSickLeaveBalance
END