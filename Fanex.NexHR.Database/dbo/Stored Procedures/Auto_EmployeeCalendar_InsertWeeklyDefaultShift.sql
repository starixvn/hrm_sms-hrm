

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

	IF @FromDate IS NULL
	BEGIN
		SET @FromDate = GETDATE()
	END

	IF @ToDate IS NULL
	BEGIN
		SET @ToDate = DATEADD(MONTH, 4, @FromDate)
	END

	CREATE TABLE #tblDate (WorkDate DATE)

	WHILE @FromDate <= @ToDate
	BEGIN
		INSERT INTO #tblDate (WorkDate)
		VALUES (@FromDate)

		SET @FromDate = DATEADD(DAY, 1, @FromDate)
	END

	INSERT INTO dbo.EmployeeCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsOffDay
		,IsHoliday
		,IsPublic
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
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,1
	FROM dbo.EmployeeDefaultShift AS shift WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = shift.EmpID
		AND emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
		AND empsum.IsDefaultSchedule = 1
		,#tblDate AS tblc
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON tblc.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
	WHERE (
			@EmployeeId IS NULL
			OR emp.EmpId = @EmployeeId
			)
		AND (
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

	EXEC dbo.ScheduleArrangement_UpdateOffDayReplacement @ActorId = - 1
		,@EmployeeIds = @EmployeeId
		,@FromDate = @FromDate
		,@ToDate = @ToDate
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Insert weekly default shift unsuccessfully!'
	END

	DROP TABLE #tblDate
END
