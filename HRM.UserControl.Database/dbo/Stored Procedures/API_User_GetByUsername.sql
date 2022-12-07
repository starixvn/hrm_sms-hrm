
CREATE PROCEDURE [dbo].[API_User_GetByUsername] @Username NVARCHAR(50)
AS
/*
Created: 20211014@Tim.Trinh
Task: get user by username
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT u.UserId
		,u.Username
		,u.Nickname
		,u.FullName
		,u.Code
		,u.[Password]
		,u.[OldPassword]
		,u.Email
		,u.IsActive
		,u.IsDeleted
	FROM dbo.[User] AS u WITH (NOLOCK)
	WHERE u.Username = @Username
		OR u.Code = @Username
END