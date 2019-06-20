/****** Object:  Stored Procedure dbo.Department_Get    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Get    Script Date: 7/19/2016 5:22:11 PM ******/


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

	IF(@FromDate IS NOT NULL)
	BEGIN
		SELECT @FromDate = DATEADD(HH, -1, @FromDate)
	END

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

