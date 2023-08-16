
CREATE PROCEDURE [dbo].[Manpower_AnnualRequestForm_Update] @RequestId INT
	,@DetailXml NVARCHAR(MAX)
	,@StatusId TINYINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230620@Tim.Trinh
Task: update annual manpower forecast request form
DB:
Original:

Revisions:
Param's Explanation:
	- @DetailXml: "<Root><Row SiteId='' SectionId='' JobLevel='' LocalAddition='' ExpatAddition='' /></Root>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 3
	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@DocHandler INT

	CREATE TABLE #tmpDetail (
		SiteId SMALLINT
		,SectionId INT
		,SectionName NVARCHAR(50)
		,JobLevel NVARCHAR(50)
		,LocalAddition SMALLINT
		,ExpatAddition SMALLINT
		)

	CREATE TABLE #tmpDetailLog (
		SiteId SMALLINT
		,SectionId INT
		,SectionName NVARCHAR(50)
		,JobLevel NVARCHAR(50)
		,LocalAddition SMALLINT
		,ExpatAddition SMALLINT
		)

	IF ISNULL(@DetailXml, '') != ''
	BEGIN
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DetailXml

		INSERT INTO #tmpDetail (
			SiteId
			,SectionId
			,SectionName
			,JobLevel
			,LocalAddition
			,ExpatAddition
			)
		SELECT SiteId
			,SectionId
			,SectionName
			,JobLevel
			,LocalAddition
			,ExpatAddition
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (SiteId SMALLINT, SectionId INT, SectionName NVARCHAR(50), JobLevel NVARCHAR(50), LocalAddition SMALLINT, ExpatAddition SMALLINT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,UnitName AS '@UnitName'
				,StatusId AS '@StatusId'
				,VerifiedBy AS '@VerifiedBy'
				,ApprovedBy AS '@ApprovedBy'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
				,(
					SELECT SiteId AS '@SiteId'
						,SectionId AS '@SectionId'
						,SectionName AS '@SectionName'
						,JobLevel AS '@JobLevel'
						,LocalAddition AS '@LocalAddition'
						,ExpatAddition AS '@ExpatAddition'
					FROM dbo.Manpower_AnnualRequestFormDetail WITH (NOLOCK)
					WHERE RequestId = @RequestId
					FOR XML PATH('z')
						,TYPE
					)
			FROM dbo.Manpower_AnnualRequestForm WITH (NOLOCK)
			WHERE RequestId = @RequestId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	DELETE rqdt
	OUTPUT DELETED.SiteId
		,DELETED.SectionId
		,DELETED.SectionName
		,DELETED.JobLevel
		,DELETED.LocalAddition
		,DELETED.ExpatAddition
	INTO #tmpDetailLog(SiteId, SectionId, SectionName, JobLevel, LocalAddition, ExpatAddition)
	FROM dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (ROWLOCK, UPDLOCK)
	WHERE rqdt.RequestId = @RequestId
		AND NOT EXISTS (
			SELECT 1
			FROM #tmpDetail AS tmp WITH (NOLOCK)
			WHERE rqdt.SiteId = tmp.SiteId
				AND rqdt.SectionId = tmp.SectionId
				AND rqdt.JobLevel = tmp.JobLevel
			)

	UPDATE rqdt
	SET rqdt.LocalAddition = tmp.LocalAddition
		,rqdt.ExpatAddition = tmp.ExpatAddition
	OUTPUT INSERTED.SiteId
		,INSERTED.SectionId
		,INSERTED.SectionName
		,INSERTED.JobLevel
		,INSERTED.LocalAddition
		,INSERTED.ExpatAddition
	INTO #tmpDetailLog(SiteId, SectionId, SectionName, JobLevel, LocalAddition, ExpatAddition)
	FROM dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpDetail AS tmp WITH (NOLOCK) ON rqdt.SiteId = tmp.SiteId
		AND rqdt.SectionId = tmp.SectionId
		AND rqdt.JobLevel = tmp.JobLevel
	WHERE rqdt.RequestId = @RequestId
		AND (
			rqdt.LocalAddition != tmp.LocalAddition
			OR rqdt.ExpatAddition != tmp.ExpatAddition
			)

	INSERT INTO dbo.Manpower_AnnualRequestFormDetail (
		RequestId
		,SiteId
		,SectionId
		,SectionName
		,JobLevel
		,LocalAddition
		,ExpatAddition
		)
	OUTPUT INSERTED.SiteId
		,INSERTED.SectionId
		,INSERTED.SectionName
		,INSERTED.JobLevel
		,INSERTED.LocalAddition
		,INSERTED.ExpatAddition
	INTO #tmpDetailLog(SiteId, SectionId, SectionName, JobLevel, LocalAddition, ExpatAddition)
	SELECT @RequestId
		,SiteId
		,SectionId
		,SectionName
		,JobLevel
		,LocalAddition
		,ExpatAddition
	FROM #tmpDetail AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK)
			WHERE rqdt.RequestId = @RequestId
				AND rqdt.SiteId = tmp.SiteId
				AND rqdt.SectionId = tmp.SectionId
				AND rqdt.JobLevel = tmp.JobLevel
			)
		AND (
			tmp.LocalAddition > 0
			OR tmp.ExpatAddition > 0
			)

	UPDATE rq
	SET rq.StatusId = @StatusId
		,rq.VerifiedBy = CASE 
			WHEN @StatusId = @VerifiedStatusId THEN @ModifiedBy
			ELSE rq.VerifiedBy
			END
		,rq.ApprovedBy = CASE 
			WHEN @StatusId = @ApprovedStatusId THEN @ModifiedBy
			ELSE rq.ApprovedBy
			END
		,rq.ModifiedBy = @ModifiedBy
		,rq.ModifiedDate = @startRun
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (ROWLOCK, UPDLOCK)
	WHERE rq.RequestId = @RequestId

	INSERT INTO dbo.Manpower_AnnualRequestFormDetail_Log (
		RequestId
		,SiteId
		,SectionId
		,SectionName
		,JobLevel
		,LocalAddition
		,ExpatAddition
		,ModifiedBy
		,ModifiedDate
		)
	SELECT @RequestId
		,SiteId
		,SectionId
		,SectionName
		,JobLevel
		,LocalAddition
		,ExpatAddition
		,@ModifiedBy
		,@startRun
	FROM #tmpDetailLog WITH (NOLOCK)

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,UnitName AS '@UnitName'
				,StatusId AS '@StatusId'
				,VerifiedBy AS '@VerifiedBy'
				,ApprovedBy AS '@ApprovedBy'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
				,(
					SELECT SiteId AS '@SiteId'
						,SectionId AS '@SectionId'
						,SectionName AS '@SectionName'
						,JobLevel AS '@JobLevel'
						,LocalAddition AS '@LocalAddition'
						,ExpatAddition AS '@ExpatAddition'
					FROM dbo.Manpower_AnnualRequestFormDetail WITH (NOLOCK)
					WHERE RequestId = @RequestId
					FOR XML PATH('z')
						,TYPE
					)
			FROM dbo.Manpower_AnnualRequestForm WITH (NOLOCK)
			WHERE RequestId = @RequestId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = NULL
		,@SpName = 'Manpower_AnnualRequestForm_UpdateRequest'
		,@ObjectId = @RequestId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration

	DROP TABLE #tmpDetail
END