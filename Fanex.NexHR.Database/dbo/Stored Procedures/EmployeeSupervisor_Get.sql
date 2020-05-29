
CREATE PROCEDURE [dbo].[EmployeeSupervisor_Get] @EmployeeId INT = NULL
	,@ActionType TINYINT = NULL
	,@GroupIds NVARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
AS
/*
Created: 20140717@Quinn.Vo
Task: Get list supervisors.
DB: SMS-HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblGroup (GroupId INT)

	CREATE TABLE #tblEmployeeSupervisor (EmpId INT)

	CREATE TABLE #tblEmployee (
		EmpId INT
		,OwnerId INT
		,ActionType TINYINT
		)

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN		
		INSERT INTO #tblGroup (GroupId)
		SELECT val
		FROM fnc_strSplit(@GroupIds, ',')
	END

	INSERT INTO #tblEmployee (
		EmpId
		,OwnerId
		,ActionType
		)
	SELECT DISTINCT su.EmpId
		,su.OwnerId
		,su.ActionType
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS [owner] WITH (NOLOCK) ON su.OwnerId = [owner].EmpId
		AND ISNULL([owner].isDeleted, 0) = 0
		AND [owner].[Enabled] = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON su.EmpId = emp.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON emp.EmpId = es.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN #tblGroup AS tblgr WITH (NOLOCK) ON gr.GroupId = tblgr.GroupId
	WHERE ISNULL(su.IsDeleted, 0) = 0
		AND su.ActionType IN (2, 3, 8)
		AND (
			@ActionType IS NULL
			OR su.ActionType = @ActionType
			)
		AND ISNULL(@EmployeeId, emp.EmpId) = emp.EmpId
		AND ISNULL(@IsLocal, es.IsLocal) = es.IsLocal
		AND (
			ISNULL(@GroupIds, '') = ''
			OR tblgr.GroupId IS NOT NULL
			)

	INSERT INTO #tblEmployee (
		EmpId
		,OwnerId
		,ActionType
		)
	SELECT DISTINCT su.OwnerId
		,su.EmpId
		,su.ActionType
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS [owner] WITH (NOLOCK) ON su.EmpId = [owner].EmpId
		AND ISNULL([owner].isDeleted, 0) = 0
		AND [owner].[Enabled] = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON su.OwnerId = emp.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON emp.EmpId = es.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN #tblGroup AS tblgr WITH (NOLOCK) ON gr.GroupId = tblgr.GroupId
	WHERE ISNULL(su.IsDeleted, 0) = 0
		AND su.ActionType IN (4, 5)
		AND (
			@ActionType IS NULL
			OR su.ActionType = @ActionType
			)
		AND ISNULL(@EmployeeId, emp.EmpId) = emp.EmpId
		AND ISNULL(@IsLocal, es.IsLocal) = es.IsLocal
		AND (
			ISNULL(@GroupIds, '') = ''
			OR tblgr.GroupId IS NOT NULL
			)

	INSERT INTO #tblEmployeeSupervisor(EmpId)
	SELECT DISTINCT e.EmpId
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.EmployeeSupervisor AS su WITH (NOLOCK) ON e.EmpId = su.EmpId
		LEFT JOIN #tblGroup AS tblgr WITH (NOLOCK) ON e.GroupId = tblgr.GroupId
	WHERE (
			@ActionType IS NULL
			OR su.ActionType = @ActionType
			)
		AND ISNULL(@EmployeeId, e.EmpId) = e.EmpId
		AND ISNULL(@IsLocal, es.IsLocal) = es.IsLocal
		AND (
			ISNULL(@GroupIds, '') = ''
			OR tblgr.GroupId IS NOT NULL
			)
		AND e.[Enabled] = 1
		AND ISNULL(e.IsDeleted, 0) = 0
		AND ISNULL(su.IsDeleted, 0) = 0
	ORDER BY e.EmpId

	SELECT DISTINCT e.EmpId
		,e.Code
		,e.NickName
		,e.FullName
		,g.GroupId AS TeamId
		,g.Code
		,g.[Name]
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN #tblEmployeeSupervisor AS esu WITH (NOLOCK) ON esu.EmpId = e.EmpId
		LEFT JOIN #tblGroup AS tblgr WITH (NOLOCK) ON g.GroupId = tblgr.GroupId
	WHERE ISNULL(e.isDeleted, 0) = 0
		AND ISNULL(e.[Enabled], 1) = 1
		AND ISNULL(@EmployeeId, e.EmpId) = e.EmpId
		AND ISNULL(@IsLocal, es.IsLocal) = es.IsLocal
		AND (
			ISNULL(@GroupIds, '') = ''
			OR tblgr.GroupId IS NOT NULL
			)
	ORDER BY e.Code

	SELECT emRo.EmployeeId
		,emRo.RoleId
		,loDa.Code
		,loDa.[Name]
	FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
		INNER JOIN dbo.LookupData AS loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
		INNER JOIN dbo.[Lookup] AS look WITH (NOLOCK) ON look.Id = loDa.LookupId
		AND look.Code = 'SR' -- System Role
		INNER JOIN #tblEmployeeSupervisor AS e WITH (NOLOCK) ON e.EmpId = emRo.EmployeeId;

	SELECT DISTINCT su.OwnerId
		,e.NickName
		,e.FullName
		,su.ActionType
		,su.EmpId
		,e1.NickName
		,e1.FullName
		,e1.Code
	FROM #tblEmployee AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.OwnerId = e.EmpId
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		INNER JOIN dbo.Employee AS e1 WITH (NOLOCK) ON su.EmpId = e1.EmpId
		AND ISNULL(e1.isDeleted, 0) = 0
		AND e1.[Enabled] = 1
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e1.EmpId = es.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON e1.GroupId = gr.GroupId
	ORDER BY e1.Code
		,e.NickName
		,su.ActionType

	DROP TABLE #tblEmployee

	DROP TABLE #tblGroup
	
	DROP TABLE #tblEmployeeSupervisor
END