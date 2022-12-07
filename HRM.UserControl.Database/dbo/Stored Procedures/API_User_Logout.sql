
CREATE PROCEDURE [dbo].[API_User_Logout] @OnUserId BIGINT
AS
/*
Created: 202220728@Tim.Trinh
Task: user logout
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM dbo.OnUser WITH (ROWLOCK, UPDLOCK)
	WHERE Id = @OnUserId
END