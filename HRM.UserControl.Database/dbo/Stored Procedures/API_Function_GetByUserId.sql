
CREATE PROCEDURE [dbo].[API_Function_GetByUserId] @UserId INT
	,@SiteId SMALLINT
AS
/*
Created: 20211027@Tim.Trinh
Task: get function by userid
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
		,CAST(MAX(CAST(rolefnc.IsRead AS TINYINT)) AS BIT) AS IsRead
		,CAST(MAX(CAST(rolefnc.IsHost AS TINYINT)) AS BIT) AS IsHost
	FROM dbo.[Function] AS fnc WITH (NOLOCK)
		INNER JOIN dbo.RoleFunction AS rolefnc WITH (NOLOCK) ON fnc.SiteId = rolefnc.SiteId
		AND fnc.FunctionId = rolefnc.FunctionId
		AND (
			rolefnc.IsRead = 1
			OR rolefnc.IsHost = 1
			)
		INNER JOIN dbo.UserRole AS urole WITH (NOLOCK) ON rolefnc.SiteId = urole.SiteId
		AND rolefnc.RoleId = urole.RoleId
		INNER JOIN dbo.[User] AS u WITH (NOLOCK) ON urole.UserId = u.UserId
	WHERE u.UserId = @UserId
		AND fnc.SiteId = @SiteId
		AND fnc.IsActive = 1
	GROUP BY fnc.FunctionId
		,fnc.FunctionName
		,fnc.FunctionKey
END