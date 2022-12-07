
CREATE PROCEDURE [dbo].[API_Role_GetByUserId] @UserId INT
	,@SiteId SMALLINT
AS
/*
Created: 20211110@Tim.Trinh
Task: get role by userid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT r.RoleId
		,r.Code
		,r.[Name]
		,r.IsActive
		,r.IsDefault
	FROM dbo.[Role] AS r WITH (NOLOCK)
		INNER JOIN dbo.UserRole AS ur WITH (NOLOCK) ON r.RoleId = ur.RoleId
		AND r.SiteId = ur.SiteId
	WHERE ur.UserId = @UserId
		AND r.SiteId = @SiteId
		AND r.IsActive = 1
END