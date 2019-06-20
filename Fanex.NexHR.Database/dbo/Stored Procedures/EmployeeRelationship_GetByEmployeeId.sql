/****** Object:  Stored Procedure dbo.EmployeeGroup_Get    Script Date: 7/20/2016 10:19:06 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeGroup_Get    Script Date: 7/19/2016 5:22:14 PM ******/

CREATE PROCEDURE [dbo].[EmployeeRelationship_GetByEmployeeId] 
	@EmployeeId INT
AS
/*
Created: 20190611@Oliver.Dang
Task: Get relationship by employeeId
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT r.Id as RelationshipId
		  ,r.EmpId
		  ,r.FullName
		  ,r.Phone
		  ,r.Gender
		  ,r.DateOfBirth
		  ,r.CreatedBy
		  ,r.CreatedBy
		  ,r.ModifiedBy
		  ,r.ModifiedDate
			
	FROM dbo.EmployeeRelationship as r WITH(NOLOCK)
	WHERE r.EmpId = @EmployeeId
	ORDER BY r.CreatedDate
	
END