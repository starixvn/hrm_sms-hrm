
CREATE PROCEDURE [dbo].[Function_Insert] @FunctionId INT OUTPUT
	,@SiteId SMALLINT
	,@FunctionKey NVARCHAR(100)
	,@FunctionName NVARCHAR(100)
	,@Description NVARCHAR(200) = NULL
	,@ParentId INT = NULL
	,@Policy TINYINT = NULL
	,@IsActive BIT
	,@DisplayOrder SMALLINT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 2021119@teddy.le
Task: create function
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
			WHERE FunctionName = @FunctionName
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
		,@xml XML

	INSERT INTO dbo.[Function] (
		SiteId
		,FunctionKey
		,FunctionName
		,[Description]
		,ParentId
		,Policy
		,IsActive
		,DisplayOrder
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@SiteId
		,@FunctionKey
		,@FunctionName
		,@Description
		,@ParentId
		,@Policy
		,@IsActive
		,@DisplayOrder
		,@CreatedBy
		,@startRun
		)

	SET @FunctionId = SCOPE_IDENTITY()
	-- write log
	SET @xml = (
			SELECT @FunctionId AS '@FunctionId'
				,@SiteId AS '@SiteId'
				,@FunctionKey AS '@FunctionKey'
				,@FunctionName AS '@FunctionName'
				,@Description AS '@Description'
				,@ParentId AS '@ParentId'
				,@Policy AS '@Policy'
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
		,@SpName = 'Function_Insert'
		,@ObjectId = @FunctionId
		,@Data = @logData
		,@OldData = NULL
		,@LogDate = @startRun
		,@Duration = @duration
END