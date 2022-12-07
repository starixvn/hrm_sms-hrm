
CREATE PROCEDURE [dbo].[Site_Update] @SiteId SMALLINT
	,@CompanyId SMALLINT
	,@SiteName NVARCHAR(50)
	,@SiteURL NVARCHAR(100) = NULL
	,@IsActive BIT
	,@ModifiedBy INT
	,@AccessToken VARCHAR(100)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211019@Tim.Trinh
Task: update site
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT SiteId
			FROM dbo.[Site] WITH (NOLOCK)
			WHERE SiteId != @SiteId
				AND SiteName = @SiteName
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Site Name already exists.'

		RETURN;
	END

	IF EXISTS (
			SELECT SiteId
			FROM dbo.[Site] WITH (NOLOCK)
			WHERE SiteId != @SiteId
				AND AccessToken = @AccessToken
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Access Token already exists.'

		RETURN;
	END

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT SiteId AS '@SiteId'
				,CompanyId AS '@CompanyId'
				,SiteName AS '@SiteName'
				,IsActive AS '@IsActive'
				,AccessToken AS '@AccessToken'
				,SiteURL AS '@SiteURL'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Site] AS s WITH (NOLOCK)
			WHERE s.SiteId = @SiteId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE dbo.[Site] WITH (ROWLOCK, UPDLOCK)
	SET CompanyId = @CompanyId
		,SiteName = @SiteName
		,AccessToken = AccessToken
		,IsActive = @IsActive
		,SiteURL = @SiteURL
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE SiteId = @SiteId

	-- write log
	SET @xml = (
			SELECT SiteId AS '@SiteId'
				,CompanyId AS '@CompanyId'
				,SiteName AS '@SiteName'
				,IsActive AS '@IsActive'
				,AccessToken AS '@AccessToken'
				,SiteURL AS '@SiteURL'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Site] AS s WITH (NOLOCK)
			WHERE s.SiteId = @SiteId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Site_Update'
		,@ObjectId = @SiteId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END