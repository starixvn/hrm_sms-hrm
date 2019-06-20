/****** Object:  Stored Procedure dbo.Department_GetByParentId    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_GetByParentId    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Department_GetByParentId]
	@ParentId INT = NULL
	,@IsActive BIT = NULL
AS
/*
Created: 20151222@Quinn.Vo
Task: Get departments by parent id.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT dep.[DepartmentId]
		,dep.[Code]
		,dep.[Name]
		,dep.IsActive
		,dep.[ModifiedDate]
		,dep.[CreatedDate]
		,dep.[ParentId]
		,dep2.[Name]
		,dep.[ManagerId]
		,mg.FullName
		,mg.NickName
		--,dep.[CreatedBy]
		--,cr.FullName
		--,cr.NickName
		--,dep.[ModifiedBy]
		--,md.FullName
		--,md.NickName
	FROM dbo.Department AS dep WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS cr WITH (NOLOCK) ON dep.CreatedBy = cr.EmpId
	LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.EmpId
	LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON dep.[ModifiedBy] = md.EmpId
	LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
	WHERE (
			@ParentId IS NULL
			OR dep.ParentId = @ParentId
			)
		AND (
			@IsActive IS NULL
			OR dep.IsActive = @IsActive
			)
END
