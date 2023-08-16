
CREATE PROCEDURE [dbo].[Manpower_Period_Insert] @PeriodId SMALLINT OUTPUT
	,@Name NVARCHAR(200)
	,@IsActive BIT = 1
	,@SettingXml VARCHAR(MAX)
	,@CreatedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2023-06-07@Brian.Nguyen
Task: insert period
DB: bodb_IUC_Local
Original:

Revisions:
Param's Explanation:
	@SettingXml: <Root><Row ProcessId='' FromDate='' ToDate='' /></Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Year INT = YEAR(GETDATE()) + 1

	IF EXISTS (
			SELECT 1
			FROM dbo.Manpower_Period WITH (NOLOCK)
			WHERE [Year] = @Year
				AND IsActive = 1
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'' + CAST(@Year AS NVARCHAR(10)) + ' Manpower Forecast exists.'

		RETURN
	END

	DECLARE @logData NVARCHAR(MAX)
		,@xml XML
		,@startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@DocHandler INT

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

	INSERT INTO dbo.Manpower_Period (
		[Name]
		,[Year]
		,IsActive
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@Name
		,@Year
		,@IsActive
		,@CreatedBy
		,@startRun
		)

	SET @PeriodId = SCOPE_IDENTITY()

	INSERT INTO dbo.Manpower_PeriodProcessSetting (
		PeriodId
		,ProcessId
		,FromDate
		,ToDate
		)
	SELECT @PeriodId
		,ProcessId
		,FromDate
		,ToDate
	FROM #tmpProcessSetting WITH (NOLOCK)

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,[Name] AS '@Name'
				,IsActive AS '@IsActive'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
				,(
					SELECT ProcessId AS '@ProcessId'
						,FromDate AS '@FromDate'
						,ToDate AS '@ToDate'
					FROM #tmpProcessSetting WITH (NOLOCK)
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

	EXEC dbo.Log_Detail_Insert @UserId = @CreatedBy
		,@SiteId = NULL
		,@SpName = 'Manpower_Period_Insert'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@OldData = NULL
		,@LogDate = @startRun
		,@Duration = @duration

	DROP TABLE #tmpProcessSetting
END