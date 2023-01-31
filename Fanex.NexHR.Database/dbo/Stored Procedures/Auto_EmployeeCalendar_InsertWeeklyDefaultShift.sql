
CREATE PROCEDURE [dbo].[Auto_EmployeeCalendar_InsertWeeklyDefaultShift] @EmployeeId INT = NULL
	,@FromDate DATE = NULL
	,@ToDate DATE = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-05-14
Task: insert weekly default shift
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @tempDate DATE
	DECLARE @tblDate TABLE (WorkDate DATE)

	IF @FromDate IS NULL
	BEGIN
		SET @FromDate = GETDATE()
	END

	IF @ToDate IS NULL
	BEGIN
		SET @ToDate = DATEADD(DAY, 6, @FromDate)
	END

	SET @tempDate = @FromDate

	WHILE @tempDate <= @ToDate
	BEGIN
		INSERT INTO @tblDate (WorkDate)
		VALUES (@tempDate)

		SET @tempDate = DATEADD(DAY, 1, @tempDate)
	END

	INSERT INTO dbo.EmployeeCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsOffDay
		,IsFixedOffDay
		,IsHoliday
		,IsPublic
		,ModifiedDate
		)
	SELECT DISTINCT emp.EmpId
		,tblc.WorkDate
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN NULL
			WHEN DATEPART(dw, tblc.WorkDate) = 1 THEN shift.SunShift
			WHEN DATEPART(dw, tblc.WorkDate) = 2 THEN shift.MonShift
			WHEN DATEPART(dw, tblc.WorkDate) = 3 THEN shift.TueShift
			WHEN DATEPART(dw, tblc.WorkDate) = 4 THEN shift.WedShift
			WHEN DATEPART(dw, tblc.WorkDate) = 5 THEN shift.ThuShift
			WHEN DATEPART(dw, tblc.WorkDate) = 6 THEN shift.FriShift
			WHEN DATEPART(dw, tblc.WorkDate) = 7 THEN shift.SatShift END
			) AS ShiftId
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 0
			WHEN DATEPART(dw, tblc.WorkDate) = 1
				AND shift.SunShift IS NULL THEN 1
			WHEN DATEPART(dw, tblc.WorkDate) = 2
				AND shift.MonShift IS NULL THEN 1
			WHEN DATEPART(dw, tblc.WorkDate) = 3
				AND shift.TueShift IS NULL THEN 1
			WHEN DATEPART(dw, tblc.WorkDate) = 4
				AND shift.WedShift IS NULL THEN 1
			WHEN DATEPART(dw, tblc.WorkDate) = 5
				AND shift.ThuShift IS NULL THEN 1
			WHEN DATEPART(dw, tblc.WorkDate) = 6
				AND shift.FriShift IS NULL THEN 1
			WHEN DATEPART(dw, tblc.WorkDate) = 7
				AND shift.SatShift IS NULL THEN 1
			ELSE 0 END
			) AS IsOffDay
		,0 AS IsFixedOffDay
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,1 AS IsPublic
		,GETDATE() AS ModifiedDate
	FROM dbo.EmployeeDefaultShift AS shift WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = shift.EmpId
		AND (
			@EmployeeId IS NULL
			OR emp.EmpId = @EmployeeId
			)
		AND emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
		AND empsum.IsDefaultSchedule = 1
		,@tblDate AS tblc
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON tblc.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
	WHERE (
			shift.SunShift IS NOT NULL
			OR shift.MonShift IS NOT NULL
			OR shift.TueShift IS NOT NULL
			OR shift.WedShift IS NOT NULL
			OR shift.ThuShift IS NOT NULL
			OR shift.FriShift IS NOT NULL
			OR shift.SatShift IS NOT NULL
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
			WHERE emp.EmpId = empca.EmpId
				AND tblc.WorkDate = empca.WorkDate
			)

	UPDATE empca
	SET empca.ShiftId = (CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN NULL
			WHEN DATEPART(dw, empca.WorkDate) = 1 THEN shift.SunShift
			WHEN DATEPART(dw, empca.WorkDate) = 2 THEN shift.MonShift
			WHEN DATEPART(dw, empca.WorkDate) = 3 THEN shift.TueShift
			WHEN DATEPART(dw, empca.WorkDate) = 4 THEN shift.WedShift
			WHEN DATEPART(dw, empca.WorkDate) = 5 THEN shift.ThuShift
			WHEN DATEPART(dw, empca.WorkDate) = 6 THEN shift.FriShift
			WHEN DATEPART(dw, empca.WorkDate) = 7 THEN shift.SatShift END
			)
		,empca.IsOffDay = (CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 0
			WHEN DATEPART(dw, empca.WorkDate) = 1
				AND shift.SunShift IS NULL THEN 1
			WHEN DATEPART(dw, empca.WorkDate) = 2
				AND shift.MonShift IS NULL THEN 1
			WHEN DATEPART(dw, empca.WorkDate) = 3
				AND shift.TueShift IS NULL THEN 1
			WHEN DATEPART(dw, empca.WorkDate) = 4
				AND shift.WedShift IS NULL THEN 1
			WHEN DATEPART(dw, empca.WorkDate) = 5
				AND shift.ThuShift IS NULL THEN 1
			WHEN DATEPART(dw, empca.WorkDate) = 6
				AND shift.FriShift IS NULL THEN 1
			WHEN DATEPART(dw, empca.WorkDate) = 7
				AND shift.SatShift IS NULL THEN 1
			ELSE 0 END
			)
		,empca.IsHoliday = (CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			)
		,empca.IsPublic = 1
		,empca.ModifiedDate = GETDATE()
	FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empca.EmpId = emp.EmpId
		AND emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND (
			@EmployeeId IS NULL
			OR emp.EmpId = @EmployeeId
			)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
		AND empsum.IsDefaultSchedule = 1
		INNER JOIN dbo.EmployeeDefaultShift AS shift WITH (NOLOCK) ON emp.EmpId = shift.EmpId
		AND (
			shift.SunShift IS NOT NULL
			OR shift.MonShift IS NOT NULL
			OR shift.TueShift IS NOT NULL
			OR shift.WedShift IS NOT NULL
			OR shift.ThuShift IS NOT NULL
			OR shift.FriShift IS NOT NULL
			OR shift.SatShift IS NOT NULL
			)
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON empca.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
	WHERE empca.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND empca.ShiftId IS NULL
		AND empca.IsHoliday = 0
		AND empca.IsOffDay = 0
		AND empca.IsLeaveDay = 0

	EXEC dbo.ScheduleArrangement_UpdateOffDayReplacement @ActorId = - 1
		,@EmployeeIds = @EmployeeId
		,@FromDate = @FromDate
		,@ToDate = @ToDate
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT
END
