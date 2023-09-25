
CREATE PROCEDURE [dbo].[Manpower_Period_Update] @PeriodId SMALLINT
	,@Name NVARCHAR(200)
	,@SettingXml VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2023-06-08@Brian.Nguyen
Task: update period
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
	@SettingXml: <Root><Row ProcessId='' FromDate='' ToDate='' /></Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 3
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@Today DATE = GETDATE()
	DECLARE @DocHandler INT

	CREATE TABLE #tmpProcessSetting (
		ProcessId TINYINT
		,FromDate DATE
		,ToDate DATE
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@SettingXml

	INSERT INTO #tmpProcessSetting (
		ProcessId
		,FromDate
		,ToDate
		)
	SELECT ProcessId
		,FromDate
		,ToDate
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (ProcessId TINYINT, FromDate DATE, ToDate DATE) AS data

	EXEC sp_xml_removedocument @DocHandler

	---- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
				,(
					SELECT ProcessId AS '@ProcessId'
						,FromDate AS '@FromDate'
						,ToDate AS '@ToDate'
					FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
					WHERE PeriodId = @PeriodId
					FOR XML PATH('z')
						,TYPE
					)
			FROM dbo.Manpower_Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.Manpower_Period WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE PeriodId = @PeriodId

	UPDATE rq
	SET rq.StatusId = CASE 
			WHEN rq.StatusId != @PendingStatusId
				AND ps.ProcessId = 1
				AND @Today BETWEEN ps.FromDate
					AND ps.ToDate THEN @PendingStatusId
			ELSE rq.StatusId
			END
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Manpower_PeriodProcessSetting AS ps WITH (NOLOCK) ON rq.PeriodId = ps.PeriodId
	WHERE ps.PeriodId = @PeriodId

	UPDATE ps
	SET ps.FromDate = tmp.FromDate
		,ps.ToDate = tmp.ToDate
	FROM dbo.Manpower_PeriodProcessSetting AS ps WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpProcessSetting AS tmp WITH (NOLOCK) ON ps.ProcessId = tmp.ProcessId
	WHERE ps.PeriodId = @PeriodId
		AND (
			ps.FromDate != tmp.FromDate
			OR ps.ToDate != tmp.ToDate
			)

	---- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
				,(
					SELECT ProcessId AS '@ProcessId'
						,FromDate AS '@FromDate'
						,ToDate AS '@ToDate'
					FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
					WHERE PeriodId = @PeriodId
					FOR XML PATH('z')
						,TYPE
					)
			FROM dbo.Manpower_Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = NULL
		,@SpName = 'Manpower_Period_Update'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration

	DROP TABLE #tmpProcessSetting
END