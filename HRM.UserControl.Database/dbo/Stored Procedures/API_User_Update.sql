
CREATE PROCEDURE [dbo].[API_User_Update] @UserId INT
	,@SiteId SMALLINT
	,@RefId INT = NULL
	,@Code VARCHAR(20) = NULL
	,@Nickname NVARCHAR(50) = NULL
	,@FullName NVARCHAR(100) = NULL
	,@Email VARCHAR(50) = NULL
	,@IsActive BIT
	,@OldPassword VARCHAR(100) = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211108@Tim.Trinh
Task: update user info
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@tmpUserId INT

	SELECT @tmpUserId = UserId
	FROM dbo.UserMapping WITH (NOLOCK)
	WHERE SiteId = @SiteId
		AND NewUserId = @UserId

	SET @tmpUserId = ISNULL(@tmpUserId, @UserId)

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,Code AS '@Code'
				,Nickname AS '@Nickname'
				,FullName AS '@FullName'
				,Email AS '@Email'
				,IsActive AS '@IsActive'
				,OldPassword AS '@OldPassword'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @tmpUserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.[User] WITH (ROWLOCK, UPDLOCK)
	SET Code = @Code
		,Nickname = @Nickname
		,FullName = @FullName
		,Email = @Email
		,IsActive = @IsActive
		,[Password] = CASE WHEN ISNULL(@OldPassword, '') != '' THEN NULL ELSE [Password] END
		,OldPassword = CASE WHEN ISNULL(@OldPassword, '') != '' THEN @OldPassword ELSE OldPassword END
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE UserId = @tmpUserId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.UserSite WITH (NOLOCK)
			WHERE UserId = @tmpUserId
				AND SiteId = @SiteId
			)
	BEGIN
		INSERT INTO dbo.UserSite (
			UserId
			,SiteId
			,IsActive
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@tmpUserId
			,@SiteId
			,@IsActive
			,@ModifiedBy
			,@startRun
			)
	END

	IF @RefId IS NOT NULL
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.UserMapping WITH (NOLOCK)
			WHERE RefId = @RefId
				AND SiteId = @SiteId
			)
	BEGIN
		INSERT INTO dbo.UserMapping (
			UserId
			,SiteId
			,RefId
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@tmpUserId
			,@SiteId
			,@RefId
			,@ModifiedBy
			,@startRun
			)
	END

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,Code AS '@Code'
				,Nickname AS '@Nickname'
				,FullName AS '@FullName'
				,Email AS '@Email'
				,IsActive AS '@IsActive'
				,OldPassword AS '@OldPassword'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @tmpUserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'API_User_Update'
		,@ObjectId = @tmpUserId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END