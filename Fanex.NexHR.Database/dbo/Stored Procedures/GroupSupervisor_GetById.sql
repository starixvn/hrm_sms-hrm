
CREATE PROCEDURE [dbo].[GroupSupervisor_GetById] @TeamId INT
AS
/*
Created: 20140723@Quinn.Vo
Task: Get list supervisors by group.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Supervisors TABLE (
		GroupId INT
		,ActionType TINYINT
		,Supervisors NVARCHAR(MAX)
		)
	DECLARE @SupervisorActions TABLE (
		GroupId INT
		,ActionType TINYINT
		)

	INSERT INTO @SupervisorActions (
		GroupId
		,ActionType
		)
	VALUES (
		@TeamId
		,2
		) -- verify

	INSERT INTO @SupervisorActions (
		GroupId
		,ActionType
		)
	VALUES (
		@TeamId
		,3
		) -- approve

	INSERT INTO @SupervisorActions (
		GroupId
		,ActionType
		)
	VALUES (
		@TeamId
		,4
		) -- shift

	INSERT INTO @SupervisorActions (
		GroupId
		,ActionType
		)
	VALUES (
		@TeamId
		,5
		) -- on behalf request

	INSERT INTO @SupervisorActions (
		GroupId
		,ActionType
		)
	VALUES (
		@TeamId
		,8
		) -- edit discipline

	INSERT INTO @Supervisors (
		GroupId
		,ActionType
		,Supervisors
		)
	SELECT es.GroupId
		,es.ActionType
		,STUFF((
				SELECT CAST(', ' AS VARCHAR(MAX)) + a.NickName
				FROM (
					SELECT DISTINCT su.GroupId
						,ActionType
						,e.NickName
					FROM [dbo].GroupSupervisor AS su WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.OwnerId = e.EmpId
					WHERE su.GroupId = @TeamId
						AND su.ActionType IN (2, 3, 4, 5, 8)
						AND ISNULL(su.IsDeleted, 0) = 0
					) AS a
				WHERE a.GroupId = es.GroupId
					AND a.ActionType = es.ActionType
				FOR XML PATH('')
				), 1, 1, '') AS Supervisor
	FROM @SupervisorActions AS es
	ORDER BY es.GroupId
		,es.ActionType

	SELECT g.GroupId
		,g.Code
		,g.[Name]
		,sp.ActionType
		,sp.Supervisors
	FROM @Supervisors AS sp
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON sp.GroupId = g.GroupId
	ORDER BY sp.GroupId
		,sp.ActionType
END
