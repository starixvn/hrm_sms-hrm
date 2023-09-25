
CREATE PROCEDURE [dbo].[Manpower_GetUnit]
AS
/*  
Created: 20230905@Brian.Nguyen
Task: get unit
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT unit.[Name] AS UnitName
	FROM dbo.HRM_Unit AS unit WITH (NOLOCK)
	WHERE unit.IsActive = 1
		AND unit.IsDeleted = 0
END