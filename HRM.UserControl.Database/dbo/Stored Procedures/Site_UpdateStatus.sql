
CREATE PROCEDURE [dbo].[Site_UpdateStatus] @SiteId SMALLINT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211019@Tim.Trinh
Task: update status of site
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

	SET @xml = (
			SELECT SiteId AS '@SiteId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Site] AS s WITH (NOLOCK)
			WHERE s.SiteId = @SiteId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE dbo.[Site] WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE SiteId = @SiteId

	-- write log
	SET @xml = (
			SELECT SiteId AS '@SiteId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Site] AS s WITH (NOLOCK)
			WHERE s.SiteId = @SiteId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(VARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Site_UpdateStatus'
		,@ObjectId = @SiteId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END