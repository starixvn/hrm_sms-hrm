
CREATE PROCEDURE [dbo].[Menu_UpdateStatus] @MenuItemId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211129@Teddy.le
Task: update status of menu
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
		,@SiteId SMALLINT

	SELECT @SiteId = SiteId
	FROM dbo.[MenuItem] WITH (NOLOCK)
	WHERE MenuItemId = @MenuItemId

	-- write log
	SET @xml = (
			SELECT MenuItemId AS '@MenuItemId'
				,@SiteId AS '@SiteId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS 'ModifiedDate'
			FROM dbo.[MenuItem] WITH (NOLOCK)
			WHERE MenuItemId = @MenuItemId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.[MenuItem] WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE MenuItemId = @MenuItemId

	-- write log
	SET @xml = (
			SELECT MenuItemId AS '@MenuItemId'
				,@SiteId AS '@SiteId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS 'ModifiedDate'
			FROM dbo.[MenuItem] AS menu WITH (NOLOCK)
			WHERE menu.MenuItemId = @MenuItemId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Menu_UpdateStatus'
		,@ObjectId = @MenuItemId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END