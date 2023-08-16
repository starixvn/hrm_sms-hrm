
CREATE PROCEDURE [dbo].[Manpower_AnnualRequestForm_UpdateStatusMultiRequest] @RequestIds NVARCHAR(MAX)
	,@StatusId TINYINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230718@Brian.Nguyen
Task: update status
DB:
Original:

Revisions:
Param's Explanation:
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

	CREATE TABLE #tmpRequest (RequestId INT PRIMARY KEY)

	INSERT INTO #tmpRequest (RequestId)
	SELECT val
	FROM dbo.fnc_strSplit(@RequestIds, ',')
	WHERE ISNUMERIC(val) = 1

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,StatusId AS '@StatusId'
				,VerifiedBy AS '@VerifiedBy'
				,ApprovedBy AS '@ApprovedBy'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.Manpower_AnnualRequestForm AS request WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM #tmpRequest AS tmp WITH (NOLOCK)
					WHERE request.RequestId = tmp.RequestId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

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
	WHERE EXISTS (
			SELECT 1
			FROM #tmpRequest AS tmp WITH (NOLOCK)
			WHERE rq.RequestId = tmp.RequestId
			)

	-- write log
	SET @xml = (
			SELECT PeriodId AS '@PeriodId'
				,StatusId AS '@StatusId'
				,VerifiedBy AS '@VerifiedBy'
				,ApprovedBy AS '@ApprovedBy'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM #tmpRequest AS tmp WITH (NOLOCK)
					WHERE rq.RequestId = tmp.RequestId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = NULL
		,@SpName = 'Manpower_AnnualRequestForm_UpdateStatusMultiRequest'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration

	DROP TABLE #tmpRequest
END