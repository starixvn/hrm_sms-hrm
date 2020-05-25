CREATE PROCEDURE [dbo].[Unit_Get] @ParentId INT = NULL
	,@IsActive BIT = NULL
AS
/*
Created: 20200521@Patsy.Doan
Task: Get unit.
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SELECT u.UnitId
		,u.Code
		,u.Name
		,u.IsActive
		,u.ModifiedDate
		,null as ManagerId
	FROM dbo.Unit as u
	WHERE (
			@IsActive IS NULL
			OR u.IsActive = @IsActive
			)
	ORDER BY u.Code
END


