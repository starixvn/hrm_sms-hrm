
CREATE PROCEDURE [dbo].[Site_Insert] @SiteId SMALLINT OUTPUT
	,@CompanyId SMALLINT
	,@SiteName VARCHAR(50)
	,@SiteURL NVARCHAR(100) = NULL
	,@IsActive BIT
	,@AccessToken VARCHAR(100)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211019@Tim.Trinh
Task: create site
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
			WHERE SiteName = @SiteName
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Site Name already exists.'

		RETURN;
	END

	IF EXISTS (
			SELECT SiteId
			FROM dbo.[Site] WITH (NOLOCK)
			WHERE AccessToken = @AccessToken
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
		,@xml XML

	INSERT INTO dbo.[Site] (
		CompanyId
		,SiteName
		,IsActive
		,SiteURL
		,AccessToken
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@CompanyId
		,@SiteName
		,@IsActive
		,@SiteURL
		,@AccessToken
		,@CreatedBy
		,@startRun
		)

	SET @SiteId = SCOPE_IDENTITY()
	SET @xml = (
			SELECT @SiteId AS '@SiteId'
				,@CompanyId AS '@CompanyId'
				,@SiteName AS '@SiteName'
				,@IsActive AS '@IsActive'
				,@AccessToken AS '@AccessToken'
				,@SiteURL AS '@SiteURL'
				,@CreatedBy AS '@CreatedBy'
				,@startRun AS '@CreatedDate'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @CreatedBy
		,@SiteId = @SiteId
		,@SpName = 'Site_Insert'
		,@ObjectId = @SiteId
		,@Data = @logData
		,@OldData = NULL
		,@LogDate = @startRun
		,@Duration = @duration
END