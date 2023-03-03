
CREATE PROCEDURE [dbo].[API_UnitHead_UpdateSchedule] @SiteId SMALLINT
	,@Value VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*  
Created: 20230227@Tim.Trinh
Task: update unit head schedule
DB:
Original:
Param's Explanation:
	• @Value:
		<Root>
			<Row EmpId="1" IsWorkDay="true" IsOffDay="false" IsHoliday="false" IsLeaveDay="false" LeaveTypeCode="" LeaveTypeName="" />
		</Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData VARCHAR(MAX)
		,@logDataOld VARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tempUnitHeadSchedule (
		EmpId INT
		,WorkDate DATE
		,IsWorkDay BIT DEFAULT(0)
		,IsOffDay BIT DEFAULT(0)
		,IsHoliday BIT DEFAULT(0)
		,IsLeaveDay BIT DEFAULT(0)
		,LeaveTypeCode NVARCHAR(50)
		,LeaveTypeName NVARCHAR(200)
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tempUnitHeadSchedule (
		EmpId
		,WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
		)
	SELECT EmpId
		,WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmpId INT, WorkDate DATE, IsWorkDay BIT, IsOffDay BIT, IsHoliday BIT, IsLeaveDay BIT, LeaveTypeCode NVARCHAR(50), LeaveTypeName NVARCHAR(200)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	-- write log
	SET @xml = (
			SELECT uhs.SiteId AS '@SiteId'
				,uhs.EmpId AS '@EmpId'
				,uhs.WorkDate AS '@WorkDate'
				,uhs.IsWorkDay AS '@IsWorkDay'
				,uhs.IsOffDay AS '@IsOffDay'
				,uhs.IsHoliday AS '@IsHoliday'
				,uhs.IsLeaveDay AS '@IsLeaveDay'
				,uhs.LeaveTypeCode AS '@LeaveTypeCode'
				,uhs.LeaveTypeName AS '@LeaveTypeName'
			FROM dbo.UnitHeadSchedule AS uhs WITH (NOLOCK)
				INNER JOIN #tempUnitHeadSchedule AS tmp WITH (NOLOCK) ON uhs.EmpId = tmp.EmpId
				AND uhs.WorkDate = tmp.WorkDate
			WHERE uhs.SiteId = @SiteId
				AND (
					uhs.IsWorkDay != tmp.IsWorkDay
					OR uhs.IsOffDay != tmp.IsOffDay
					OR uhs.IsHoliday != tmp.IsHoliday
					OR uhs.IsLeaveDay != tmp.IsLeaveDay
					OR uhs.LeaveTypeCode != tmp.LeaveTypeCode
					OR uhs.LeaveTypeName != tmp.LeaveTypeName
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE uhs
	SET uhs.IsWorkDay = tmp.IsWorkDay
		,uhs.IsOffDay = tmp.IsOffDay
		,uhs.IsHoliday = tmp.IsHoliday
		,uhs.IsLeaveDay = tmp.IsLeaveDay
		,uhs.LeaveTypeCode = tmp.LeaveTypeCode
		,uhs.LeaveTypeName = tmp.LeaveTypeName
		,uhs.SyncDate = @startRun
	FROM dbo.UnitHeadSchedule AS uhs WITH (NOLOCK)
		INNER JOIN #tempUnitHeadSchedule AS tmp WITH (NOLOCK) ON uhs.EmpId = tmp.EmpId
		AND uhs.WorkDate = tmp.WorkDate
	WHERE uhs.SiteId = @SiteId
		AND (
			uhs.IsWorkDay != tmp.IsWorkDay
			OR uhs.IsOffDay != tmp.IsOffDay
			OR uhs.IsHoliday != tmp.IsHoliday
			OR uhs.IsLeaveDay != tmp.IsLeaveDay
			OR uhs.LeaveTypeCode != tmp.LeaveTypeCode
			OR uhs.LeaveTypeName != tmp.LeaveTypeName
			)

	INSERT INTO dbo.UnitHeadSchedule (
		EmpId
		,SiteId
		,WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
		,SyncDate
		)
	SELECT tmp.EmpId
		,@SiteId
		,tmp.WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
		,@startRun
	FROM #tempUnitHeadSchedule AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.UnitHeadSchedule AS uhs WITH (NOLOCK)
			WHERE uhs.EmpId = tmp.EmpId
				AND uhs.SiteId = @SiteId
				AND uhs.WorkDate = tmp.WorkDate
			)

	-- write log
	SET @xml = (
			SELECT @SiteId AS '@SiteId'
				,EmpId AS '@EmpId'
				,WorkDate AS '@WorkDate'
				,IsWorkDay AS '@IsWorkDay'
				,IsOffDay AS '@IsOffDay'
				,IsHoliday AS '@IsHoliday'
				,IsLeaveDay AS '@IsLeaveDay'
				,LeaveTypeCode AS '@LeaveTypeCode'
				,LeaveTypeName AS '@LeaveTypeName'
			FROM #tempUnitHeadSchedule WITH (NOLOCK)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	--EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
	--	,@SiteId = @SiteId
	--	,@SpName = 'API_UnitHead_UpdateSchedule'
	--	,@ObjectId = NULL
	--	,@Data = @logData
	--	,@OldData = @logDataOld
	--	,@LogDate = @startRun
	--	,@Duration = @duration
	DROP TABLE #tempUnitHeadSchedule
END