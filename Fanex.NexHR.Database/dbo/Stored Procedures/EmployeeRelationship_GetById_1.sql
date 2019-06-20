/****** Object:  Stored Procedure dbo.Department_GetById    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_GetById    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[EmployeeRelationship_GetById]
	@RelationshipId INT
AS
/*
Created: 20190429@Oliver.Dang
Task: Get relationship by Id.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT r.Id As RelationshipId
		  ,r.FullName
		  ,r.Phone
		  ,r.DateOfBirth
		  ,r.Gender
	FROM dbo.EmployeeRelationship As r WITH (NOLOCK)
	WHERE r.Id = @RelationshipId

END