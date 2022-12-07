
CREATE PROCEDURE [dbo].[Function_Update] @FunctionId INT
	,@SiteId SMALLINT
	,@FunctionKey NVARCHAR(100)
	,@FunctionName NVARCHAR(100)
	,@Description NVARCHAR(200) = NULL
	,@ParentId INT = NULL
	,@Policy TINYINT = NULL
	,@IsActive BIT
	,@DisplayOrder SMALLINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211119@Teddy.Le
Task: update function
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT FunctionId
			FROM dbo.[Function] WITH (NOLOCK)
			WHERE FunctionId != @FunctionId
				AND FunctionName = @FunctionName
				AND SiteId = @SiteId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Function Name already exists.'

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
			SELECT FunctionId AS '@FunctionId'
				,SiteId AS '@SiteId'
				,FunctionKey AS '@FunctionKey'
				,FunctionName AS '@FunctionName'
				,[Description] AS '@Description'
				,ParentId AS '@ParentId'
				,Policy AS '@Policy'
				,IsActive AS '@IsActive'
				,DisplayOrder AS '@DisplayOrder'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Function] WITH (NOLOCK)
			WHERE FunctionId = @FunctionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.[Function] WITH (ROWLOCK, UPDLOCK)
	SET SiteId = @SiteId
		,FunctionKey = @FunctionKey
		,FunctionName = @FunctionName
		,[Description] = @Description
		,ParentId = @ParentId
		,Policy = @Policy
		,IsActive = @IsActive
		,DisplayOrder = @DisplayOrder
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE FunctionId = @FunctionId

	--write log
	SET @xml = (
			SELECT FunctionId AS '@FunctionId'
				,SiteId AS '@SiteId'
				,FunctionKey AS '@FunctionKey'
				,FunctionName AS '@FunctionName'
				,[Description] AS '@Description'
				,ParentId AS '@ParentId'
				,Policy AS '@Policy'
				,IsActive AS '@IsActive'
				,DisplayOrder AS '@DisplayOrder'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[Function] AS fnc WITH (NOLOCK)
			WHERE fnc.FunctionId = @FunctionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Function_Update'
		,@ObjectId = @FunctionId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END