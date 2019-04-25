
CREATE PROCEDURE [dbo].[ScheduleArrangement_UpdateWeekly] @ActorId INT
	,@WeeklyScheduleXml VARCHAR(MAX) = ''
	,@DefaultShiftXml VARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-04-16
Task: Update weekly schedule
DB: HRM
Original:

Revisions:
Param's Explanation:
    @WeeklyScheduleXml=N'<ROOT><Row EmployeeId="1" WorkDate="2013-01-01" ShiftId="1" IsHoliday="1" IsOffDay="1" IsPublic="1" /></ROOT>
    @DefaultShiftXml=N'<ROOT><Row EmployeeId="1" FromDate="2013-01-01" ShiftId="1" /></ROOT>

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATE = GETDATE()
		,@ModifiedDate DATETIME = GETDATE()
		,@DocHandler INT
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	CREATE TABLE #tblWeeklySchedule (
		EmployeeId INT
		,WorkDate DATE
		,ShiftId INT NULL
		,IsHoliday BIT
		,IsOffDay BIT
		,IsPublic BIT
		)

	CREATE TABLE #tblDefaultShift (
		EmployeeId INT
		,FromDate DATE
		,ShiftId INT NULL
		)

	CREATE TABLE #OTMonthlyRequest (
		RequestId BIGINT
		,NormalHours DECIMAL(18, 2)
		,OffHours DECIMAL(18, 2)
		,PublicHours DECIMAL(18, 2)
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@WeeklyScheduleXml

		INSERT INTO #tblWeeklySchedule (
			EmployeeId
			,WorkDate
			,ShiftId
			,IsHoliday
			,IsOffDay
			,IsPublic
			)
		SELECT EmployeeId
			,WorkDate
			,CASE 
				WHEN ShiftId = '' THEN NULL
				ELSE CONVERT(INT, ShiftId)
				END
			,ISNULL(IsHoliday, 0)
			,ISNULL(IsOffDay, 0)
			,ISNULL(IsPublic, 0)
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmployeeId INT, WorkDate DATE, ShiftId VARCHAR(100), IsHoliday BIT, IsOffDay BIT, IsPublic BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DefaultShiftXml

		INSERT INTO #tblDefaultShift (
			EmployeeId
			,FromDate
			,ShiftId
			)
		SELECT EmployeeId
			,FromDate
			,CASE 
				WHEN ShiftId = '' THEN NULL
				ELSE CONVERT(INT, ShiftId)
				END
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmployeeId INT, FromDate DATE, ShiftId VARCHAR(100)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	DELETE tblweekly
	FROM #tblWeeklySchedule AS tblweekly
		INNER JOIN dbo.EmployeeAttendance AS ea WITH (NOLOCK) ON tblweekly.WorkDate = ea.AttendanceDate
		AND tblweekly.EmployeeId = ea.EmpId
		INNER JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON ec.WorkDate = tblweekly.WorkDate
		AND ec.EmpId = tblweekly.EmployeeId
		AND ISNULL(ec.ShiftId, - 1) = ISNULL(tblweekly.ShiftId, - 1)
		AND ec.IsOffDay = tblweekly.IsOffDay
		AND ec.IsHoliday = tblweekly.IsHoliday
		AND ec.IsPublic = tblweekly.IsPublic
	WHERE tblweekly.WorkDate <= @Today

	DELETE tblweekly
	FROM #tblWeeklySchedule AS tblweekly
		INNER JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON tblweekly.WorkDate = ot.OvertimeDate
		AND tblweekly.EmployeeId = ot.EmpId
	WHERE tblweekly.WorkDate <= @Today
		AND ot.[Status] IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)

	DELETE tblweekly
	FROM #tblWeeklySchedule AS tblweekly
		INNER JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON tblweekly.WorkDate = ec.WorkDate
		AND tblweekly.EmployeeId = ec.EmpId
	WHERE (
			ec.IsLeaveDay = 1
			AND ec.LeaveDay = 1
			)
		OR ec.ChangeShiftId IS NOT NULL
		OR (
			ec.IsKeepInView = 1
			AND tblweekly.ShiftId IS NULL
			)
		OR (
			ec.IsKeepInView = 1
			AND tblweekly.IsHoliday = 0
			AND tblweekly.IsOffDay = 0
			)

	BEGIN TRY
		INSERT INTO #OTMonthlyRequest (
			RequestId
			,NormalHours
			,OffHours
			,PublicHours
			)
		SELECT ot.RequestId
			,SUM(ISNULL(NormalHours, 0))
			,SUM(ISNULL(OffHours, 0))
			,SUM(ISNULL(PublicHours, 0))
		FROM dbo.OTDailyRequest AS ot WITH (NOLOCK)
			INNER JOIN #tblWeeklySchedule AS tblweekly ON ot.EmpId = tblweekly.EmployeeId
			AND ot.OvertimeDate = tblweekly.WorkDate
		GROUP BY ot.RequestId

		DELETE ot
		FROM dbo.OTDailyRequest AS ot WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblWeeklySchedule AS tblweekly ON ot.EmpId = tblweekly.EmployeeId
			AND ot.OvertimeDate = tblweekly.WorkDate

		UPDATE otm
		SET otm.NormalHours = ISNULL(otm.NormalHours, 0) - ISNULL(ot.NormalHours, 0)
			,otm.OffHours = ISNULL(otm.OffHours, 0) - ISNULL(ot.OffHours, 0)
			,otm.PublicHours = ISNULL(otm.PublicHours, 0) - ISNULL(ot.PublicHours, 0)
		FROM dbo.OTMonthlyRequest AS otm WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #OTMonthlyRequest AS ot ON otm.RequestId = ot.RequestId

		INSERT INTO dbo.tmpEmployeeAttendace (
			EmpId
			,WorkDate
			,InsertedDate
			)
		SELECT tblweekly.EmployeeId
			,tblweekly.WorkDate
			,@ModifiedDate
		FROM #tblWeeklySchedule AS tblweekly WITH (NOLOCK)
			LEFT JOIN dbo.tmpEmployeeAttendace AS att WITH (NOLOCK) ON tblweekly.EmployeeId = att.EmpId
			AND tblweekly.WorkDate = att.WorkDate
		WHERE att.EmpId IS NULL
			AND DATEDIFF(DD, tblweekly.WorkDate, GETDATE()) >= 0

		UPDATE ec
		SET ec.ShiftId = tblweekly.ShiftId
			,ec.ModifiedDate = @ModifiedDate
			,ec.ModifiedBy = @ActorId
			,ec.IsOffDay = tblweekly.IsOffDay
			,ec.IsHoliday = tblweekly.IsHoliday
			,ec.IsPublic = tblweekly.IsPublic
		FROM dbo.EmployeeCalendar AS ec WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblWeeklySchedule AS tblweekly WITH (NOLOCK) ON tblweekly.WorkDate = ec.WorkDate
			AND tblweekly.EmployeeId = ec.EmpId

		DELETE tblweekly
		FROM #tblWeeklySchedule AS tblweekly
			INNER JOIN dbo.EmployeeCalendar AS ec ON tblweekly.WorkDate = ec.WorkDate
			AND tblweekly.EmployeeId = ec.EmpId

		INSERT INTO dbo.EmployeeCalendar (
			WorkDate
			,EmpId
			,ShiftId
			,IsOffDay
			,IsHoliday
			,IsPublic
			,ModifiedBy
			,ModifiedDate
			)
		SELECT WorkDate
			,EmployeeId
			,ShiftId
			,IsOffDay
			,IsHoliday
			,IsPublic
			,@ActorId
			,@ModifiedDate
		FROM #tblWeeklySchedule

		UPDATE ds
		SET ds.ShiftId = tblds.ShiftId
			,ds.ModifiedBy = @ActorId
			,ds.ModifiedDate = @ModifiedDate
		FROM dbo.ScheduleArrangementDefaultShift AS ds WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblDefaultShift AS tblds ON ds.FromDate = tblds.FromDate
			AND ds.EmployeeId = tblds.EmployeeId
			AND ISNULL(ds.ShiftId, -1) <> ISNULL(tblds.ShiftId, -1)

		DELETE tblds
		FROM #tblDefaultShift AS tblds
		WHERE EXISTS (
				SELECT 1
				FROM dbo.ScheduleArrangementDefaultShift AS ds WITH (NOLOCK)
				WHERE tblds.EmployeeId = ds.EmployeeId
					AND tblds.FromDate = ds.FromDate
				)

		INSERT INTO dbo.ScheduleArrangementDefaultShift (
			EmployeeId
			,FromDate
			,ShiftId
			,ModifiedBy
			,ModifiedDate
			)
		SELECT EmployeeId
			,FromDate
			,ShiftId
			,@ActorId
			,@ModifiedDate
		FROM #tblDefaultShift
		WHERE ShiftId IS NOT NULL
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Updated unsuccessfully!'

		RETURN
	END CATCH;

	DROP TABLE #tblWeeklySchedule

	DROP TABLE #tblDefaultShift

	DROP TABLE #OTMonthlyRequest
END