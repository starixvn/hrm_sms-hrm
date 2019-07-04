
CREATE PROCEDURE [dbo].[EmployeeSupervisor_GetByQuery] @EmpId INT
	,@Role TINYINT = NULL
	,@RequestType TINYINT = NULL
	,@query NVARCHAR(256) = ''
	,@includeLoginUser BIT = 1
AS
/*
Created: Quinn.Vo@2013/11/11
Task: Get list supervisor
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ShiftManagerRoleId TINYINT = 4
		,@OnBehalfRequestRoleId TINYINT = 5
	DECLARE @q NVARCHAR(258) = '%' + ISNULL(@query, '') + '%'

	CREATE TABLE #tblSupervisors (
		OwnerId INT
		,Nickname NVARCHAR(50)
		,FullName NVARCHAR(100)
		)

	INSERT INTO #tblSupervisors (
		OwnerId
		,Nickname
		,FullName
		)
	SELECT EmpId
		,NickName
		,FullName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE @includeLoginUser = 1
		AND EmpId = @EmpId

	INSERT INTO #tblSupervisors (
		OwnerId
		,Nickname
		,FullName
		)
	SELECT DISTINCT OwnerId
		,emp.NickName
		,emp.FullName
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON es.OwnerId = emp.EmpId
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
	WHERE es.EmpId = @EmpId
		AND ISNULL(es.IsDeleted, 0) = 0
		AND es.ActionType IN (@ShiftManagerRoleId, @OnBehalfRequestRoleId)
		AND (
			@Role IS NULL
			OR es.ActionType = @Role
			)
		AND (
			@RequestType IS NULL
			OR es.RequestType = @RequestType
			)
		AND (
			emp.NickName LIKE @q
			OR emp.FullName LIKE @q
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisors AS tbl
			WHERE emp.EmpId = tbl.OwnerId
			)

	INSERT INTO #tblSupervisors (
		OwnerId
		,Nickname
		,FullName
		)
	SELECT DISTINCT emp.EmpId
		,emp.NickName
		,emp.FullName
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON es.EmpId = emp.EmpId
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
	WHERE es.OwnerId = @EmpId
		AND ISNULL(es.IsDeleted, 0) = 0
		AND es.ActionType NOT IN (@ShiftManagerRoleId, @OnBehalfRequestRoleId)
		AND (
			@Role IS NULL
			OR es.ActionType = @Role
			)
		AND (
			@RequestType IS NULL
			OR es.RequestType = @RequestType
			)
		AND (
			emp.NickName LIKE @q
			OR emp.FullName LIKE @q
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisors AS tbl
			WHERE emp.EmpId = tbl.OwnerId
			)

	INSERT INTO #tblSupervisors (
		OwnerId
		,Nickname
		,FullName
		)
	SELECT DISTINCT emp.EmpId
		,emp.NickName
		,emp.FullName
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gs.GroupId = emp.GroupId
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
	WHERE gs.OwnerId = @EmpId
		AND ISNULL(gs.IsDeleted, 0) = 0
		AND (
			@Role IS NULL
			OR gs.ActionType = @Role
			)
		AND (
			@RequestType IS NULL
			OR gs.RequestType = @RequestType
			)
		AND (
			emp.NickName LIKE @q
			OR emp.FullName LIKE @q
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisors AS tbl
			WHERE emp.EmpId = tbl.OwnerId
			)

	SELECT OwnerId
		,Nickname
		,FullName
	FROM #tblSupervisors
	ORDER BY Nickname

	DROP TABLE #tblSupervisors
END
