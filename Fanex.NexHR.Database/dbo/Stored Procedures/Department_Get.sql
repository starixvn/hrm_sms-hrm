

CREATE PROCEDURE [dbo].[Department_Get]
	@FromDate DATETIME = NULL
AS
/*
Created: 20150828@Quinn.Vo
Task: Get all departments from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
	  [DepartmentId]
      ,[Code]
      ,[Name]
      ,[ParentId]
      ,[ManagerId]
      ,[DepartmentLevel]
      ,[IsActive]
      ,ISNULL([ModifiedDate],[CreatedDate]) AS ModifiedDate
	FROM dbo.[Department] WITH (NOLOCK)
	WHERE @FromDate IS NULL
	OR ModifiedDate > @FromDate
	OR CreatedDate > @FromDate
END

