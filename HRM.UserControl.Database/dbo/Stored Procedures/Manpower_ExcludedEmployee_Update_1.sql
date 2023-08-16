
CREATE PROCEDURE [dbo].[Manpower_ExcludedEmployee_Update] @SiteId SMALLINT
	,@EmpId INT
	,@IsExcluded BIT = 0
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20230531@Tim.Trinh
Task: update exclude employee
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
				,EmpId AS '@EmpId'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.Manpower_ExcludedEmployee WITH (NOLOCK)
			WHERE SiteId = @SiteId
				AND EmpId = @EmpId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	IF @IsExcluded = 1
	BEGIN
		IF NOT EXISTS (
				SELECT 1
				FROM dbo.Manpower_ExcludedEmployee WITH (NOLOCK)
				WHERE SiteId = @SiteId
					AND EmpId = @EmpId
				)
		BEGIN
			INSERT INTO dbo.Manpower_ExcludedEmployee (
				SiteId
				,EmpId
				,ModifiedBy
				,ModifiedDate
				)
			VALUES (
				@SiteId
				,@EmpId
				,@ModifiedBy
				,@startRun
				)
		END
	END
	ELSE
	BEGIN
		DELETE dbo.Manpower_ExcludedEmployee WITH (ROWLOCK, UPDLOCK)
		WHERE SiteId = @SiteId
			AND EmpId = @EmpId
	END

	-- write log
	SET @xml = (
			SELECT SiteId AS '@SiteId'
				,EmpId AS '@EmpId'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.Manpower_ExcludedEmployee WITH (NOLOCK)
			WHERE SiteId = @SiteId
				AND EmpId = @EmpId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'Manpower_ExcludedEmployee_Update'
		,@ObjectId = @EmpId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END