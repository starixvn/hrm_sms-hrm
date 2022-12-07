
CREATE PROCEDURE [dbo].[API_Function_GetByRoleId] @SiteId SMALLINT
	,@RoleId SMALLINT
AS
/*
Created: 20211102@Tim.Trinh
Task: get function by roleid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT fnc.FunctionId
		,fnc.FunctionKey
		,fnc.FunctionName
		,fnc.Policy
		,CAST(MAX(CAST(rolefnc.IsRead AS TINYINT)) AS BIT) AS IsRead
		,CAST(MAX(CAST(rolefnc.IsHost AS TINYINT)) AS BIT) AS IsHost
	FROM dbo.[Function] AS fnc WITH (NOLOCK)
		INNER JOIN dbo.RoleFunction AS rolefnc WITH (NOLOCK) ON fnc.SiteId = rolefnc.SiteId
		AND fnc.FunctionId = rolefnc.FunctionId
		AND (
			rolefnc.IsRead = 1
			OR rolefnc.IsHost = 1
			)
		INNER JOIN dbo.[Role] AS r WITH (NOLOCK) ON rolefnc.SiteId = r.SiteId
		AND rolefnc.RoleId = r.RoleId
	WHERE r.RoleId = @RoleId
		AND fnc.SiteId = @SiteId
		AND fnc.IsActive = 1
	GROUP BY fnc.FunctionId
		,fnc.FunctionName
		,fnc.FunctionKey
		,fnc.Policy
END