
CREATE PROCEDURE [dbo].[Manpower_Period_Delete] @PeriodId SMALLINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2023-06-02@Brian.Nguyen
Task: delete period.
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logDataOld NVARCHAR(MAX)
		,@xml XML
		,@startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT

	IF EXISTS (
			SELECT 1
			FROM dbo.Manpower_AnnualRequestForm WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Can not delete this period, it''s already used.'

		RETURN
	END

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,[Year] AS '@Year'
				,IsActive AS '@IsActive'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.Manpower_Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	DELETE
	FROM dbo.Manpower_Period WITH (ROWLOCK, UPDLOCK)
	WHERE PeriodId = @PeriodId

	DELETE
	FROM dbo.Manpower_PeriodProcessSetting WITH (ROWLOCK, UPDLOCK)
	WHERE PeriodId = @PeriodId

	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = NULL
		,@SpName = 'Manpower_Period_Delete'
		,@ObjectId = @PeriodId
		,@Data = NULL
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END