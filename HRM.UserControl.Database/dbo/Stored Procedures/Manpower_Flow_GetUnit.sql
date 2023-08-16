
CREATE PROCEDURE [dbo].[Manpower_Flow_GetUnit]
AS
/*  
Created: 20230605@Tim.Trinh
Task: get unit
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT unit.[Name]
		,CASE 
			WHEN flow.UnitName IS NOT NULL THEN 1
			ELSE 0
			END AS HasFlow
	FROM dbo.HRM_Unit AS unit WITH (NOLOCK)
		LEFT JOIN (
		SELECT DISTINCT flow.UnitName
		FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		) AS flow ON unit.[Name] = flow.UnitName
	WHERE unit.IsActive = 1
		AND unit.IsDeleted = 0
END