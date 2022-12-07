
CREATE PROCEDURE [dbo].[Menu_Update] @MenuItemId SMALLINT
	,@SiteId SMALLINT
	,@FunctionId SMALLINT = NULL
	,@MenuItemName NVARCHAR(100)
	,@Description NVARCHAR(200) = NULL
	,@ParentId INT = NULL
	,@URL NVARCHAR(100)
	,@DisplayOrder SMALLINT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211129@Teddy.Le
Task: update menu
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT MenuItemId
			FROM dbo.MenuItem WITH (NOLOCK)
			WHERE MenuItemId != MenuItemId
				AND MenuItemName = @MenuItemName
				AND SiteId = @SiteId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Menu Name already exists.'

		RETURN;
	END

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- write log
	SET @xml = (
			SELECT MenuItemId AS '@MenuItemId'
				,SiteId AS '@SiteId'
				,FunctionId AS '@FunctionId'
				,MenuItemName AS '@MenuItemName'
				,[Description] AS '@Description'
				,ParentId AS '@ParentId'
				,URL AS '@URL'
				,IsActive AS '@IsActive'
				,DisplayOrder AS '@DisplayOrder'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.MenuItem WITH (NOLOCK)
			WHERE MenuItemId = @MenuItemId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.MenuItem WITH (ROWLOCK, UPDLOCK)
	SET SiteId = @SiteId
		,FunctionId = @FunctionId
		,MenuItemName = @MenuItemName
		,[Description] = @Description
		,ParentId = @ParentId
		,URL = @URL
		,IsActive = @IsActive
		,DisplayOrder = @DisplayOrder
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE MenuItemId = @MenuItemId

	-- write log
	SET @xml = (
			SELECT MenuItemId AS '@MenuItemId'
				,SiteId AS '@SiteId'
				,FunctionId AS '@FunctionId'
				,MenuItemName AS '@MenuItemName'
				,[Description] AS '@Description'
				,ParentId AS '@ParentId'
				,URL AS '@URL'
				,IsActive AS '@IsActive'
				,DisplayOrder AS '@DisplayOrder'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.MenuItem WITH (NOLOCK)
			WHERE MenuItemId = @MenuItemId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Menu_Update'
		,@ObjectId = @MenuItemId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END