
CREATE PROCEDURE [dbo].[API_User_GetById] @UserId INT
AS
/*
Created: 20210621@Oliver.Dang
Task: Get user by id
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT u.UserId
		,u.Username
		,u.Code
		,u.FullName
		,u.Nickname
		,u.[Password]
		,u.OldPassword
		,u.Email
		,u.IsActive
		,u.IsDeleted
		,u.IsResetPassword
	FROM dbo.[User] AS u WITH (NOLOCK)
	WHERE u.UserId = @UserId
END