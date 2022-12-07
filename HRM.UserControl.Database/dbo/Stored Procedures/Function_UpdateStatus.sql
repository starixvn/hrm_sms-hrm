
CREATE PROCEDURE [dbo].[Function_UpdateStatus] @FunctionId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211118@Teddy.le
Task: update status of function
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
	FROM dbo.[Function] WITH (NOLOCK)
	WHERE FunctionId = @FunctionId

	-- write log
	SET @xml = (
			SELECT FunctionId AS '@FunctionId'
				,@SiteId AS '@SiteId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS 'ModifiedDate'
			FROM dbo.[Function] AS fnc WITH (NOLOCK)
			WHERE fnc.FunctionId = @FunctionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.[Function] WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE FunctionId = @FunctionId

	-- write log
	SET @xml = (
			SELECT FunctionId AS '@FunctionId'
				,@SiteId AS '@SiteId'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS 'ModifiedDate'
			FROM dbo.[Function] WITH (NOLOCK)
			WHERE FunctionId = @FunctionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Function_UpdateStatus'
		,@ObjectId = @FunctionId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END