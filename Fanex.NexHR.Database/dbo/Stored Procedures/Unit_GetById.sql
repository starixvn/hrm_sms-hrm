CREATE PROCEDURE [dbo].[Unit_GetById]
	@UnitId INT
AS
/*
Created: 20200520@Patsy.doan
Task: Unit_GetById
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
	FROM dbo.Unit as u
	WHERE u.UnitId =@UnitId
END


