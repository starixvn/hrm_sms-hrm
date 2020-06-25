CREATE PROCEDURE [dbo].[Unit_Get]
	@IsActive BIT = NULL
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
	FROM dbo.Unit as u WITH (NOLOCK) 
	WHERE (
			@IsActive IS NULL
			OR u.IsActive = @IsActive
			)
			AND U.IsDeleted = 0
	ORDER BY u.Code
END


