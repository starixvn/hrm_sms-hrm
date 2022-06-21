
CREATE PROCEDURE [dbo].[Sync_WorkingShift_Import] @WorkingShiftXml NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200318@Lilac.Nguyen
Task: Insert Shift, update EmployeeCalendar by XML
DB:
Original:

Revisions:
Param's Explanation:
- @WorkingShiftXml: "<Root>
		<Row EmployeeID={1} ShiftCode={2} ShiftDate={3} ShiftStartTime={4} ShiftEndTime={5} LastUpdated={6} IsOffDay={7} IsPublicHoliday={8} IsInAfterDay={9} IsOutAfterDay={10} IsCrossDay={11} TotalHours={12} />
	</Root>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETDATE()
		,@Today DATE = GETDATE()
		,@EmpId INT
		,@EmployeeIds VARCHAR(MAX)
		,@FromDate DATE
		,@ToDate DATE
		,@WorkDate DATE
		,@DocHandler INT

	CREATE TABLE #tmpWorkingShift (
		EmployeeID VARCHAR(20)
		,ShiftCode VARCHAR(20)
		,ShiftDate DATE
		,CheckInTime NCHAR(5) NULL
		,CheckOutTime NCHAR(5) NULL
		,ModifiedDate DATETIME
		,IsOffDay BIT
		,IsHoliday BIT
		,IsInAfterDay BIT
		,IsOutAfterDay BIT
		,IsCrossDay BIT
		,TotalHours DECIMAL(10, 2)
		)

	CREATE TABLE #tmpEmployeeCalendar (
		EmpId INT
		,WorkDate DATE
		,ShiftId INT
		,IsOffDay BIT
		,IsHoliday BIT
		)

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@WorkingShiftXml

	INSERT INTO #tmpWorkingShift (
		EmployeeID
		,ShiftCode
		,ShiftDate
		,CheckInTime
		,CheckOutTime
		,ModifiedDate
		,IsOffDay
		,IsHoliday
		,IsInAfterDay
		,IsOutAfterDay
		,IsCrossDay
		,TotalHours
		)
	SELECT EmployeeID
		,ShiftCode
		,ShiftDate
		,CASE 
			WHEN ISNULL(CheckInTime, '') = '' THEN NULL
			ELSE CheckInTime
			END
		,CASE 
			WHEN ISNULL(CheckOutTime, '') = '' THEN NULL
			ELSE CheckOutTime
			END
		,LastUpdated
		,IsOffDay
		,IsPublicHoliday
		,IsInAfterDay
		,IsOutAfterDay
		,IsCrossDay
		,TotalHours
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmployeeID VARCHAR(20), ShiftCode VARCHAR(20), ShiftDate DATE, CheckInTime NCHAR(5), CheckOutTime NCHAR(5), LastUpdated DATETIME, IsOffDay BIT, IsPublicHoliday BIT, IsInAfterDay BIT, IsOutAfterDay BIT, IsCrossDay BIT, TotalHours DECIMAL(10, 2)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	INSERT INTO dbo.[Shift] (
		Code
		,[Name]
		,BeginCheckInTime
		,EndCheckInTime
		,BeginCheckOutTime
		,EndCheckOutTime
		,BreakTimeMins
		,TotalHours
		,CrossDay
		,IsInAfterDay
		,IsOutAfterDay
		,OWPModifiedDate
		,OWPSyncDate
		,CreatedDate
		,IsDisabled
		)
	SELECT tmpws.ShiftCode
		,tmpws.ShiftCode
		,tmpws.CheckInTime
		,tmpws.CheckInTime
		,tmpws.CheckOutTime
		,tmpws.CheckOutTime
		,0
		,tmpws.TotalHours
		,tmpws.IsCrossDay
		,tmpws.IsInAfterDay
		,tmpws.IsOutAfterDay
		,MAX(tmpws.ModifiedDate)
		,@SyncDate
		,@SyncDate
		,0
	FROM #tmpWorkingShift AS tmpws WITH (NOLOCK)
	WHERE tmpws.CheckInTime IS NOT NULL
		AND tmpws.CheckOutTime IS NOT NULL
		AND NOT EXISTS (
			SELECT s.ShiftID
			FROM dbo.[Shift] AS s WITH (NOLOCK)
			WHERE s.Code = tmpws.ShiftCode
			)
		AND EXISTS (
			SELECT emp.EmpId
			FROM dbo.Employee AS emp WITH (NOLOCK)
			WHERE tmpws.EmployeeID = emp.Code
			)
	GROUP BY tmpws.ShiftCode
		,tmpws.CheckInTime
		,tmpws.CheckOutTime
		,tmpws.IsInAfterDay
		,tmpws.IsOutAfterDay
		,tmpws.IsCrossDay
		,tmpws.TotalHours

	UPDATE ec
	SET ec.ShiftId = s.ShiftID
		,ec.IsOffDay = tmpws.IsOffDay
		,ec.IsHoliday = CASE 
			WHEN tmpws.IsHoliday = 1
				AND h.Id IS NOT NULL THEN 1
			ELSE 0
			END
		,ec.ModifiedDate = @SyncDate
		,ec.OWPModifiedDate = tmpws.ModifiedDate
		,ec.OWPSyncDate = @SyncDate
	OUTPUT INSERTED.EmpId
		,INSERTED.WorkDate
		,INSERTED.ShiftId
		,INSERTED.IsOffDay
		,INSERTED.IsHoliday
	INTO #tmpEmployeeCalendar(EmpId, WorkDate, ShiftId, IsOffDay, IsHoliday)
	FROM dbo.EmployeeCalendar AS ec WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ec.EmpId = e.EmpId
		INNER JOIN #tmpWorkingShift AS tmpws WITH (NOLOCK) ON e.Code = tmpws.EmployeeID
		AND CAST(ec.WorkDate AS DATE) = CAST(tmpws.ShiftDate AS DATE)
		INNER JOIN dbo.tmpEmployeeCalendar AS tmpca WITH (NOLOCK) ON ec.EmpId = tmpca.EmpId
		AND ec.WorkDate = tmpca.WorkDate
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Code = tmpws.ShiftCode
		LEFT JOIN dbo.Holiday AS h WITH (NOLOCK) ON tmpws.ShiftDate BETWEEN h.FromDate
			AND h.ToDate
	WHERE tmpca.ChangeShiftTypeId IS NULL
		AND tmpca.IsLeaveDay = 0
		AND tmpca.IsKeepInView = 0
		AND (
			(
				s.ShiftID IS NULL
				AND (
					(
						tmpws.IsOffDay = 1
						AND tmpca.IsOffDay = 0
						)
					OR (
						tmpws.IsOffDay = 0
						AND tmpca.IsOffDay = 1
						)
					OR (
						tmpws.IsHoliday = 1
						AND tmpca.IsHoliday = 0
						AND h.Id IS NOT NULL
						)
					OR (
						tmpws.IsHoliday = 0
						AND tmpca.IsHoliday = 1
						)
					)
				)
			OR (
				s.ShiftID IS NOT NULL
				AND tmpca.ShiftId != s.ShiftID
				)
			)

	INSERT INTO dbo.EmployeeCalendar (
		WorkDate
		,EmpId
		,ShiftId
		,IsOffDay
		,IsHoliday
		,ModifiedDate
		,OWPModifiedDate
		,OWPSyncDate
		)
	OUTPUT INSERTED.EmpId
		,INSERTED.WorkDate
		,INSERTED.ShiftId
		,INSERTED.IsOffDay
		,INSERTED.IsHoliday
	INTO #tmpEmployeeCalendar(EmpId, WorkDate, ShiftId, IsOffDay, IsHoliday)
	SELECT tmpws.ShiftDate
		,e.EmpId
		,s.ShiftID
		,CASE 
			WHEN tmpca.EmpId IS NULL
				OR (
					tmpca.EmpId IS NOT NULL
					AND tmpca.IsLeaveDay = 0
					AND tmpca.IsKeepInView = 0
					AND tmpca.ChangeShiftTypeId IS NULL
					) THEN tmpws.IsOffDay
			ELSE 0
			END
		,CASE 
			WHEN tmpws.IsHoliday = 1
				AND h.Id IS NOT NULL THEN 1
			ELSE 0
			END
		,@SyncDate
		,tmpws.ModifiedDate
		,@SyncDate
	FROM #tmpWorkingShift AS tmpws WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Code = tmpws.EmployeeID
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Code = tmpws.ShiftCode
		LEFT JOIN dbo.Holiday AS h WITH (NOLOCK) ON tmpws.ShiftDate BETWEEN h.FromDate
			AND h.ToDate
		LEFT JOIN dbo.tmpEmployeeCalendar AS tmpca WITH (NOLOCK) ON e.EmpId = tmpca.EmpId
		AND CAST(tmpws.ShiftDate AS DATE) = CAST(tmpca.WorkDate AS DATE)
	WHERE (
			(
				s.ShiftID IS NULL
				AND (
					tmpws.IsOffDay = 1
					OR (
						tmpws.IsHoliday = 1
						AND h.Id IS NOT NULL
						)
					)
				)
			OR s.ShiftID IS NOT NULL
			)
		AND NOT EXISTS (
			SELECT ec.EmpId
			FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
			WHERE ec.EmpId = e.EmpId
				AND CAST(tmpws.ShiftDate AS DATE) = CAST(ec.WorkDate AS DATE)
			)

	IF EXISTS (
			SELECT 1
			FROM #tmpEmployeeCalendar WITH (NOLOCK)
			)
	BEGIN
		SELECT @FromDate = MIN(WorkDate)
			,@ToDate = MAX(WorkDate)
		FROM #tmpEmployeeCalendar WITH (NOLOCK)

		SELECT @EmployeeIds = COALESCE(@EmployeeIds + ',', '') + CAST(EmpId AS VARCHAR(10))
		FROM #tmpEmployeeCalendar WITH (NOLOCK)

		EXEC dbo.ScheduleArrangement_UpdateOffDayReplacement @ActorId = - 1
			,@EmployeeIds = @EmployeeIds
			,@FromDate = @FromDate
			,@ToDate = @ToDate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		DECLARE @xml NVARCHAR(MAX)

		SET @xml = CAST((
					SELECT EmpId AS '@EmpId'
						,WorkDate AS '@WorkDate'
						,ShiftId AS '@ShiftId'
						,IsOffDay AS '@IsOffDay'
						,IsHoliday AS '@IsHoliday'
					FROM #tmpEmployeeCalendar WITH (NOLOCK)
					FOR XML PATH('Row')
						,ROOT('Root')
					) AS NVARCHAR(MAX))

		EXEC dbo.Log_WriteLog @LogActionId = 1004
			,@LogDate = @SyncDate
			,@Description = 'Sync working shift from OWP'
			,@TrackingValue = @xml
			,@UserId = - 1
	END

	DELETE
	FROM #tmpEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	WHERE WorkDate > @Today

	WHILE EXISTS (
			SELECT 1
			FROM #tmpEmployeeCalendar WITH (NOLOCK)
			)
	BEGIN
		SELECT TOP 1 @EmpId = EmpId
			,@WorkDate = WorkDate
		FROM #tmpEmployeeCalendar WITH (NOLOCK)

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@WorkDate
			,@WorkDate
			,1

		DELETE
		FROM #tmpEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		WHERE EmpId = @EmpId
			AND WorkDate = @WorkDate
	END

	DROP TABLE #tmpWorkingShift

	DROP TABLE #tmpEmployeeCalendar
END
