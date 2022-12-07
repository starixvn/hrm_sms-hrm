
CREATE PROCEDURE [dbo].[Role_Insert] @RoleId SMALLINT OUTPUT
	,@SiteId SMALLINT
	,@Name VARCHAR(50)
	,@IsActive BIT = 1
	,@IsDefault BIT = 0
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 2021126@Tim.Trinh
Task: create role
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
			WHERE [Name] = @Name
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
		,@xml XML
		,@maxRoleId SMALLINT

	SELECT @maxRoleId = MAX(RoleId)
	FROM dbo.[Role] WITH (NOLOCK)
	WHERE SiteId = @SiteId

	SET @RoleId = ISNULL(@maxRoleId, 0) + 1

	INSERT INTO dbo.[Role] (
		RoleId
		,SiteId
		,[Name]
		,IsActive
		,IsDefault
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@RoleId
		,@SiteId
		,@Name
		,@IsActive
		,@IsDefault
		,@CreatedBy
		,@startRun
		)

	-- Write log
	SET @xml = (
			SELECT @RoleId AS '@RoleId'
				,@SiteId AS '@SiteId'
				,@Name AS '@Name'
				,@IsActive AS '@IsActive'
				,@IsDefault AS '@IsDefault'
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
		,@SpName = 'Role_Insert'
		,@ObjectId = @RoleId
		,@Data = @logData
		,@OldData = NULL
		,@LogDate = @startRun
		,@Duration = @duration
END