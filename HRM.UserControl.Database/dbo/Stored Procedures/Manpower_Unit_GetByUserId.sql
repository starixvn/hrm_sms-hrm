
CREATE PROCEDURE [dbo].[Manpower_Unit_GetByUserId] @UserId INT
AS
/*  
Created: 20230629@Tim.Trinh
Task: get unit by userid
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @UnitHeadProcessId TINYINT = 1

	SELECT DISTINCT unit.[Name]
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON flow.UnitName = unit.[Name]
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
	WHERE flow.ProcessId = @UnitHeadProcessId
		AND EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE emp.UserId = @UserId
				AND emp.SiteId = flow.SiteId
				AND emp.EmpId = flow.EmpId
			)
END