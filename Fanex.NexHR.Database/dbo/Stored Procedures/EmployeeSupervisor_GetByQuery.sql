/****** Object:  Stored Procedure dbo.EmployeeSupervisor_GetByQuery    Script Date: 7/20/2016 10:19:07 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeSupervisor_GetByQuery    Script Date: 7/19/2016 5:22:14 PM ******/

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- [dbo].[EmployeeSupervisor_GetByQuery] 421, null, null, 'Jack'
CREATE PROCEDURE [dbo].[EmployeeSupervisor_GetByQuery] @EmpId INT
	,@Role TINYINT = NULL
	,@RequestType TINYINT = NULL
	,@query NVARCHAR(256) = ''
	,@includeLoginUser BIT = 1
AS
/*
Created: Quinn.Vo@2013/11/11
Task: Get list supervisor for Employee by EmpId, Role, Request and Query.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON

DECLARE @q NVARCHAR(258)

SET @q = '%' + @query + '%'

IF LEN(@query) = 0
	SET @query = NULL

SELECT EmpId AS OwnerId
	,FullName
	,NickName
FROM [dbo].Employee e WITH (NOLOCK)
WHERE @includeLoginUser = 1
	AND EmpId = @EmpId

UNION ALL
SELECT DISTINCT OwnerId
	,e.FullName
	,e.NickName
FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
	INNER JOIN [dbo].Employee e WITH (NOLOCK) ON sp.OwnerId = e.EmpId
WHERE sp.EmpId = @EmpId
	AND ISNULL(sp.IsDeleted, 0) = 0
	AND (
		@includeLoginUser = 0
		OR (
			@includeLoginUser = 1
			AND OwnerId != @EmpId
			)
		)
	AND (
		@Role IS NULL
		OR sp.ActionType = @Role
		)
	AND (
		@RequestType IS NULL
		OR sp.RequestType = @RequestType
		)
	AND (
		@query IS NULL
		OR NickName LIKE @q
		OR FullName LIKE @q
		OR DomainUsername LIKE @q
		OR e.Code LIKE @q
		)

UNION
SELECT DISTINCT ee.EmpId AS OwnerId
	,ee.FullName
	,ee.NickName
FROM [dbo].[GroupSupervisor] AS gs WITH (NOLOCK)
	INNER JOIN [dbo].[Employee] AS ee WITH (NOLOCK) ON gs.GroupId = ee.GroupId
WHERE gs.OwnerId = @EmpId
	AND ISNULL(gs.IsDeleted, 0) = 0
	AND (
		@Role IS NULL
		OR gs.ActionType = @Role
		)
	AND (
		@includeLoginUser = 0
		OR (
			@includeLoginUser = 1
			AND ee.EmpId != @EmpId
			)
		)
	AND (
		@RequestType IS NULL
		OR gs.RequestType = @RequestType
		)
	AND (
		@query IS NULL
		OR ee.NickName LIKE @q
		OR ee.FullName LIKE @q
		OR ee.DomainUsername LIKE @q
		OR ee.Code LIKE @q
		)
