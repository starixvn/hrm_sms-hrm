
CREATE PROCEDURE [dbo].[EmployeeSupervisor_Get] @EmployeeId INT = NULL
	,@ActionType TINYINT = NULL
	,@GroupIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
AS
/*
Created: 20140717@Quinn.Vo
Task: Get list supervisors.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Groups TABLE (GroupId INT)
	DECLARE @Supervisors TABLE (
		EmployeeId INT
		,ActionType TINYINT
		,Supervisors NVARCHAR(1000)
		)
	DECLARE @SupervisorActions TABLE (ActionType TINYINT)

	INSERT INTO @SupervisorActions (ActionType)
	VALUES (2)

	INSERT INTO @SupervisorActions (ActionType)
	VALUES (3)

	INSERT INTO @SupervisorActions (ActionType)
	VALUES (4)

	INSERT INTO @SupervisorActions (ActionType)
	VALUES (5)

	IF (@GroupIds <> '')
	BEGIN
		INSERT INTO @Groups (GroupId)
		SELECT val
		FROM fnc_strSplit(@GroupIds, ',')
	END

	INSERT INTO @Supervisors (
		EmployeeId
		,ActionType
		,Supervisors
		)
	SELECT es.EmpId AS EmployeeId
		,es.ActionType
		,STUFF((
				SELECT CAST(', ' AS VARCHAR(MAX)) + a.NickName
				FROM (
					SELECT DISTINCT su.EmpId
						,ActionType
						,e.NickName
					FROM [dbo].[EmployeeSupervisor] AS su WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.OwnerId = e.EmpId
						AND e.Enabled = 1
						AND ISNULL(e.isDeleted, 0) = 0
					WHERE su.ActionType IN (2, 3)
						AND ISNULL(su.IsDeleted, 0) = 0
						AND (
							@ActionType IS NULL
							OR ActionType = @ActionType
							)
					) AS a
				WHERE a.EmpId = es.EmpId
					AND a.ActionType = es.ActionType
				FOR XML PATH('')
				), 1, 1, '') AS Supervisor
	FROM (
		SELECT EmpId
			,ActionType
		FROM dbo.EmployeeSupervisor WITH (NOLOCK)
		WHERE ActionType IN (2, 3)
			AND ISNULL(IsDeleted, 0) = 0
			AND (
				@ActionType IS NULL
				OR ActionType = @ActionType
				)
		GROUP BY EmpId
			,ActionType
		) AS es
	ORDER BY es.EmpId
		,es.ActionType

	INSERT INTO @Supervisors (
		EmployeeId
		,ActionType
		,Supervisors
		)
	SELECT es.OwnerId AS EmployeeId
		,es.ActionType
		,STUFF((
				SELECT CAST(', ' AS VARCHAR(MAX)) + a.NickName
				FROM (
					SELECT DISTINCT OwnerId
						,ActionType
						,e.NickName
					FROM [dbo].[EmployeeSupervisor] AS su WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.EmpId = e.EmpId
						AND e.Enabled = 1
						AND ISNULL(e.isDeleted, 0) = 0
					WHERE su.ActionType IN (4, 5)
						AND ISNULL(su.IsDeleted, 0) = 0
						AND (
							@ActionType IS NULL
							OR ActionType = @ActionType
							)
					) AS a
				WHERE a.OwnerId = es.OwnerId
					AND a.ActionType = es.ActionType
				FOR XML PATH('')
				), 1, 1, '') AS Supervisor
	FROM (
		SELECT OwnerId
			,ActionType
		FROM dbo.EmployeeSupervisor WITH (NOLOCK)
		WHERE ActionType IN (4, 5)
			AND ISNULL(IsDeleted, 0) = 0
			AND (
				@ActionType IS NULL
				OR ActionType = @ActionType
				)
		GROUP BY OwnerId
			,ActionType
		) AS es
	ORDER BY es.OwnerId
		,es.ActionType

	SELECT sa.ActionType
		,ISNULL(sp.Supervisors, '') AS Supervisors
		,e.EmpId
		,e.NickName
		,e.FullName
		,e.Roles
		,g.GroupId
		,g.Code
		,g.NAME
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN @SupervisorActions AS sa ON 1 = 1
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN @Supervisors AS sp ON sp.EmployeeId = e.EmpId
		AND sa.ActionType = sp.ActionType
	WHERE (
			@ActionType IS NULL
			OR sp.ActionType = @ActionType
			)
		AND (
			@EmployeeId IS NULL
			OR e.EmpId = @EmployeeId
			)
		AND (
			@IsLocal IS NULL
			OR es.IsLocal = @IsLocal
			)
		AND (
			@GroupIds = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM @Groups
				)
			)
		AND e.Enabled = 1
		AND ISNULL(e.isDeleted, 0) = 0
	ORDER BY e.Code DESC
		,sp.ActionType
END
