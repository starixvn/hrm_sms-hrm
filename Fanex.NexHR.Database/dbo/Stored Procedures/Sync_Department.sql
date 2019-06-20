

CREATE PROCEDURE [dbo].[Sync_Department] @FromDate DATETIME = NULL
AS
/*
Created: 20171108@Tim.Trinh
Task: Get all departments from change date
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT [DepartmentId]
		,[Code]
		,[Name]
		,[ParentId]
		,[ManagerId]
		,[DepartmentLevel]
		,~[IsActive] AS IsDeleted
		,ISNULL([ModifiedDate], [CreatedDate]) AS ModifiedDate
	FROM dbo.[Department] WITH (NOLOCK)
	WHERE @FromDate IS NULL
		OR ModifiedDate > @FromDate
		OR CreatedDate > @FromDate
END