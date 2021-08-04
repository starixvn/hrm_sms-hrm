

CREATE PROCEDURE [dbo].[LeaveRequest_EmployeeCalendar_ResetOffDay] @empId INT
	,@LeaveTypeId INT
	,@fromDate DATE
	,@toDate DATE
AS
/*
Created: Tim.Trinh@2021-08-03
Task: reset off day
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OffDayShiftId SMALLINT = - 1
		,@IsDefaultSchedule TINYINT
		,@Today DATE = GETDATE()
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

	CREATE TABLE #tblResetOff (
		EmpId INT
		,WorkDate DATE
		)

	INSERT INTO #tblResetOff (
		EmpId
		,WorkDate
		)
	SELECT empca.EmpId
		,empca.WorkDate
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
	WHERE empca.EmpId = @empId
		AND empca.ShiftId IS NULL
		AND empca.IsOffDay = 1
		AND empca.WorkDate BETWEEN @Today
			AND @ToDate

	SELECT @IsDefaultSchedule = IsDefaultSchedule
	FROM dbo.EmployeeSummary AS empsum WITH (NOLOCK)
	WHERE empsum.EmpId = @empId

	IF @IsDefaultSchedule = 1
	BEGIN
		SET @fromDate = @Today

		WHILE @fromDate <= @toDate
		BEGIN
			UPDATE empca
			SET empca.ShiftId = CASE 
					WHEN DATEPART(dw, @fromDate) = 1 THEN empshift.SunShift
					WHEN DATEPART(dw, @fromDate) = 2 THEN empshift.MonShift
					WHEN DATEPART(dw, @fromDate) = 3 THEN empshift.TueShift
					WHEN DATEPART(dw, @fromDate) = 4 THEN empshift.WedShift
					WHEN DATEPART(dw, @fromDate) = 5 THEN empshift.ThuShift
					WHEN DATEPART(dw, @fromDate) = 6 THEN empshift.FriShift
					WHEN DATEPART(dw, @fromDate) = 7 THEN empshift.SatShift
					END
				,empca.IsOffDay = CASE 
					WHEN DATEPART(dw, @fromDate) = 1
						AND ISNULL(empshift.SunShift, @OffDayShiftId) = @OffDayShiftId THEN 1
					WHEN DATEPART(dw, @fromDate) = 2
						AND ISNULL(empshift.MonShift, @OffDayShiftId) = @OffDayShiftId THEN 1
					WHEN DATEPART(dw, @fromDate) = 3
						AND ISNULL(empshift.TueShift, @OffDayShiftId) = @OffDayShiftId THEN 1
					WHEN DATEPART(dw, @fromDate) = 4
						AND ISNULL(empshift.WedShift, @OffDayShiftId) = @OffDayShiftId THEN 1
					WHEN DATEPART(dw, @fromDate) = 5
						AND ISNULL(empshift.ThuShift, @OffDayShiftId) = @OffDayShiftId THEN 1
					WHEN DATEPART(dw, @fromDate) = 6
						AND ISNULL(empshift.FriShift, @OffDayShiftId) = @OffDayShiftId THEN 1
					WHEN DATEPART(dw, @fromDate) = 7
						AND ISNULL(empshift.SatShift, @OffDayShiftId) = @OffDayShiftId THEN 1
					ELSE 0
					END
			FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
				INNER JOIN dbo.EmployeeDefaultShift AS empshift WITH (NOLOCK) ON empca.EmpId = empshift.EmpId
				AND empca.EmpId = @empId
				AND empca.WorkDate = @fromDate
				INNER JOIN #tblResetOff AS resoff WITH (NOLOCK) ON empca.EmpId = resoff.EmpId
				AND empca.WorkDate = resoff.WorkDate

			SET @fromDate = DATEADD(d, 1, @fromDate)
		END
	END
	ELSE
	BEGIN
		UPDATE empca
		SET empca.ShiftId = empshift.ShiftId
			,empca.IsOffDay = 0
		FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
			INNER JOIN #tblResetOff AS resoff WITH (NOLOCK) ON empca.EmpId = resoff.EmpId
			AND empca.WorkDate = resoff.WorkDate
			INNER JOIN dbo.ScheduleArrangementDefaultShift AS empshift WITH (NOLOCK) ON empshift.EmployeeId = empca.EmpId
		WHERE empshift.ShiftId > 0
			AND empshift.FromDate <= resoff.WorkDate
	END

	DROP TABLE #tblResetOff
END