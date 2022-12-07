
CREATE PROCEDURE [dbo].[API_User_ReCheckIn] @OnUserId BIGINT
	,@IsKickedOut BIT = 0 OUTPUT
AS
/*
Created: 202220728@Tim.Trinh
Task: user recheckin
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.OnUser WITH (NOLOCK)
			WHERE Id = @OnUserId
				AND IsKickedOut = 1
			)
	BEGIN
		SET @IsKickedOut = 1
	END
	ELSE
	BEGIN
		SET @IsKickedOut = 0

		UPDATE dbo.OnUser WITH (ROWLOCK, UPDLOCK)
		SET ReloadTime = GETDATE()
		WHERE Id = @OnUserId

		IF @@ROWCOUNT = 0
		BEGIN
			SELECT @IsKickedOut = 1
		END
	END
END