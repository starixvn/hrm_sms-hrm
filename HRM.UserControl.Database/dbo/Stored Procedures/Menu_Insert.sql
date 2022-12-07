
CREATE PROCEDURE [dbo].[Menu_Insert] @MenuItemId SMALLINT OUTPUT
	,@SiteId SMALLINT
	,@FunctionId SMALLINT = NULL
	,@MenuItemName NVARCHAR(100)
	,@Description NVARCHAR(200) = NULL
	,@ParentId INT = NULL
	,@URL NVARCHAR(100)
	,@DisplayOrder SMALLINT
	,@IsActive BIT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 2021129@teddy.le
Task: create menu
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
			WHERE MenuItemName = @MenuItemName
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
		,@xml XML

	INSERT INTO dbo.MenuItem (
		SiteId
		,FunctionId
		,MenuItemName
		,[Description]
		,ParentId
		,URL
		,IsActive
		,DisplayOrder
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@SiteId
		,@FunctionId
		,@MenuItemName
		,@Description
		,@ParentId
		,@URL
		,@IsActive
		,@DisplayOrder
		,@CreatedBy
		,@startRun
		)

	SET @MenuItemId = SCOPE_IDENTITY()
	-- write log
	SET @xml = (
			SELECT @MenuItemId AS '@MenuItemId'
				,@SiteId AS '@SiteId'
				,@FunctionId AS '@FunctionId'
				,@MenuItemName AS '@MenuItemName'
				,@Description AS '@Description'
				,@ParentId AS '@ParentId'
				,@URL AS '@URL'
				,@IsActive AS '@IsActive'
				,@DisplayOrder AS '@DisplayOrder'
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
		,@SpName = 'Menu_Insert'
		,@ObjectId = @MenuItemId
		,@Data = @logData
		,@OldData = NULL
		,@LogDate = @startRun
		,@Duration = @duration
END