
CREATE PROCEDURE [dbo].[Role_Update] @SiteId SMALLINT
	,@RoleId SMALLINT
	,@Name NVARCHAR(50)
	,@IsActive BIT
	,@IsDefault BIT = 0
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211129@Tim.Trinh
Task: update role
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT RoleId
			FROM dbo.[Role] WITH (NOLOCK)
			WHERE RoleId != @RoleId
				AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Role Name already exists.'

		RETURN;
	END

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT RoleId AS '@RoleId'
				,SiteId AS '@SiteId'
				,[Name] AS '@Name'
				,IsActive AS '@IsActive'
				,IsDefault AS '@IsDefault'
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
	SET [Name] = @Name
		,IsActive = @IsActive
		,IsDefault = @IsDefault
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE SiteId = @SiteId
		AND RoleId = @RoleId

	SET @xml = (
			SELECT RoleId AS '@RoleId'
				,SiteId AS '@SiteId'
				,[Name] AS '@Name'
				,IsActive AS '@IsActive'
				,IsDefault AS '@IsDefault'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Role] WITH (NOLOCK)
			WHERE SiteId = @SiteId
				AND RoleId = @RoleId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Role_Update'
		,@ObjectId = @RoleId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END