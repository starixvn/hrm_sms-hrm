
CREATE PROCEDURE [dbo].[EmployeeSupervisor_GetDisciplineSettingById] @EmpId INT
AS
/*
Created: 20160621@Tim.Trinh
Task: Get list supervisor for Employee by EmpId.
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT su.OwnerId
		,emp.FullName
		,emp.NickName
		,emp.Email
		,ISNULL(su.OrderNo, 0) AS OrderNo
	FROM [dbo].[EmployeeSupervisor] AS su WITH (NOLOCK)
		INNER JOIN [dbo].Employee AS emp WITH (NOLOCK) ON su.OwnerId = emp.EmpId
	WHERE su.EmpId = @EmpId
		AND su.ActionType = 8 -- edit discipline
		AND su.RequestType = 0
		AND ISNULL(su.IsDeleted, 0) = 0
	ORDER BY ISNULL(su.OrderNo, 0)
END
