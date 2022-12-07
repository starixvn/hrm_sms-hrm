
CREATE PROCEDURE [dbo].[Role_UpdateStatus] @SiteId SMALLINT
	,@RoleId SMALLINT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211129@Tim.Trinh
Task: update status of role
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
				,RoleId AS '@RoleId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Role] WITH (NOLOCK)
			WHERE SiteId = @SiteId
				AND RoleId = @RoleId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE dbo.[Role] WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE SiteId = @SiteId
		AND RoleId = @RoleId

	SET @xml = (
			SELECT SiteId AS '@SiteId'
				,RoleId AS '@RoleId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Role] WITH (NOLOCK)
			WHERE SiteId = @SiteId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Role_UpdateStatus'
		,@ObjectId = @RoleId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END