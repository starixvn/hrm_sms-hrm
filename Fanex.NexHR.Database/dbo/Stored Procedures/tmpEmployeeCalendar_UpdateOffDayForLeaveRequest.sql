
CREATE PROCEDURE [dbo].[tmpEmployeeCalendar_UpdateOffDayForLeaveRequest] @requestId BIGINT = NULL
	,@empId INT
	,@leaveTypeId INT
	,@fromDate DATE
	,@toDate DATE
AS
/*
Created: Tim.Trinh@2021-08-03
Task: update off day
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tempFromDate DATE
		,@tempToDate DATE
		,@OffDaysbyWeek SMALLINT
		,@DefaultDateFirst TINYINT = @@datefirst
		-- leave type 
		,@UnpaidLeave TINYINT = 25
		,@UnpaidLeaveInsurance TINYINT = 41
		,@PaternityLeave TINYINT = 14
		,@MaternityLeave TINYINT = 12

	IF @LeaveTypeId NOT IN (@UnpaidLeave, @UnpaidLeaveInsurance, @PaternityLeave, @MaternityLeave)
		OR @ToDate < DATEADD(DAY, - 1, (DATEADD(MONTH, 1, @FromDate)))
	BEGIN
		RETURN
	END

	CREATE TABLE #tblUpdateOffDay (WorkDate DATE)

	SET DATEFIRST 1 -- monday
	SET @FromDate = DATEADD(DAY, - 1 * (6 + DATEPART(dw, @FromDate)) % 7, @FromDate)
	SET @ToDate = DATEADD(DAY, 7 - DATEPART(dw, @ToDate), @ToDate)
	SET DATEFIRST @DefaultDateFirst

	SELECT @OffDaysbyWeek = OffDaysbyWeek
	FROM dbo.EmployeeSummary WITH (NOLOCK)
	WHERE EmpId = @empId

	WHILE @FromDate < @ToDate
	BEGIN
		SET @tempFromDate = @FromDate
		SET @tempToDate = DATEADD(DAY, 6, @tempFromDate)

		DECLARE @TotalOffDaysUsed SMALLINT = 0
			,@OffDayLeft SMALLINT = 0
		DECLARE @tblOffDay TABLE (WorkDate DATE)

		SELECT @TotalOffDaysUsed = COUNT(1)
		FROM dbo.tmpEmployeeCalendar WITH (NOLOCK)
		WHERE EmpId = @empId
			AND WorkDate BETWEEN @tempFromDate
				AND @tempToDate
			AND IsOffDay = 1

		SET @OffDayLeft = @OffDaysbyWeek - @TotalOffDaysUsed

		IF @OffDayLeft > 0
		BEGIN
			INSERT INTO @tblOffDay (WorkDate)
			SELECT DISTINCT empca.WorkDate
			FROM dbo.tmpEmployeeCalendar AS empca WITH (NOLOCK)
				LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON empca.EmpId = att.EmpId
				AND empca.WorkDate = CAST(att.AttendanceDate AS DATE)
			WHERE empca.WorkDate BETWEEN @tempFromDate
					AND @tempToDate
				AND empca.EmpId = @EmpId
				AND empca.IsOffDay = 0
				AND (
					empca.IsLeaveDay = 0
					OR (
						empca.IsLeaveDay = 1
						AND empca.RequestId = @requestId
						)
					)
				AND empca.IsHoliday = 0
				AND empca.IsKeepInView = 0
				AND (
					att.EmpId IS NULL
					OR (
						att.InFixed IS NULL
						AND att.OutFixed IS NULL
						)
					)

			WHILE @OffDayLeft > 0
				AND EXISTS (
					SELECT 1
					FROM @tblOffDay
					)
			BEGIN
				DECLARE @tempWorkDate DATE

				SELECT @tempWorkDate = MAX(WorkDate)
				FROM @tblOffDay

				INSERT INTO #tblUpdateOffDay (WorkDate)
				VALUES (@tempWorkDate)

				DELETE
				FROM @tblOffDay
				WHERE WorkDate = @tempWorkDate

				SET @OffDayLeft = @OffDayLeft - 1
			END
		END

		SET @FromDate = DATEADD(DAY, 7, @FromDate)
	END

	UPDATE dbo.tmpEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	SET IsOffDay = 1
		,ShiftId = NULL
		,RequestId = NULL
		,RequestTypeId = NULL
		,LeaveTypeId = NULL
		,LeaveTypeId2 = NULL
		,IsLeaveDay = 0
		,IsLeaveOffDay = 0
		,LeaveDay = 0
		,ModifiedDate = GETDATE()
	WHERE EmpId = @empId
		AND WorkDate IN (
			SELECT WorkDate
			FROM #tblUpdateOffDay WITH (NOLOCK)
			)

	DROP TABLE #tblUpdateOffDay
END