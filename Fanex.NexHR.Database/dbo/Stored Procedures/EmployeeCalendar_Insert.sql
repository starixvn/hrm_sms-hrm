
CREATE PROCEDURE [dbo].[EmployeeCalendar_Insert] @EmpId INT = NULL
	,@fromdate DATE
	,@todate DATE
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-21
Task: insert calendar
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @MofifiedDate DATETIME = GETDATE()

	CREATE TABLE #tblDate (
		EmpId INT
		,WorkDate DATE
		,ShiftId INT
		)

	WHILE @FromDate <= @ToDate
	BEGIN
		INSERT INTO #tblDate (
			EmpId
			,WorkDate
			,ShiftId
			)
		SELECT emp.EmpId
			,@FromDate
			,CASE 
				WHEN DATEPART(dw, @FromDate) = 1 THEN SunShift
				WHEN DATEPART(dw, @FromDate) = 2 THEN MonShift
				WHEN DATEPART(dw, @FromDate) = 3 THEN TueShift
				WHEN DATEPART(dw, @FromDate) = 4 THEN WedShift
				WHEN DATEPART(dw, @FromDate) = 5 THEN ThuShift
				WHEN DATEPART(dw, @FromDate) = 6 THEN FriShift
				WHEN DATEPART(dw, @FromDate) = 7 THEN SatShift
				END
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
			AND empsum.IsDefaultSchedule = 1
			INNER JOIN dbo.EmployeeDefaultShift AS shift WITH (NOLOCK) ON emp.EmpId = shift.EmpId
		WHERE @EmpId IS NULL
			OR emp.EmpId = @EmpId

		INSERT INTO #tblDate (
			EmpId
			,WorkDate
			,ShiftId
			)
		SELECT emp.EmpId
			,@FromDate
			,ShiftId
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
			AND empsum.IsDefaultSchedule = 0
			OUTER APPLY (
			SELECT TOP 1 shift.ShiftId
			FROM dbo.ScheduleArrangementDefaultShift AS shift WITH (NOLOCK)
			WHERE shift.EmployeeId = emp.EmpId
				AND shift.ShiftId > 0
				AND shift.FromDate <= @FromDate
			ORDER BY shift.FromDate DESC
			) AS shift
		WHERE @EmpId IS NULL
			OR emp.EmpId = @EmpId

		SET @FromDate = DATEADD(DAY, 1, @FromDate)
	END

	INSERT INTO dbo.EmployeeCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsOffDay
		,IsHoliday
		,IsFixedOffDay
		,IsPublic
		,ModifiedDate
		)
	SELECT emp.EmpId
		,tblc.WorkDate
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN NULL
			ELSE tblc.ShiftId END
			) AS ShiftId
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 0
			WHEN tblc.ShiftId IS NULL THEN 1
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
		,0 AS IsFixedOffDay
		,1 AS IsPublic
		,@MofifiedDate
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		AND empsum.IsDefaultSchedule = 1
		INNER JOIN #tblDate AS tblc ON emp.EmpId = tblc.EmpId
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON tblc.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR emp.EmpId = @EmpId
			)
		AND (
			emp.ResignedDate IS NULL
			OR emp.ResignedDate > tblc.WorkDate
			)
		AND emp.JoinedDate <= tblc.WorkDate
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
			WHERE empca.EmpId = emp.EmpId
				AND empca.WorkDate = tblc.WorkDate
			)

	INSERT INTO dbo.EmployeeCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsOffDay
		,IsHoliday
		,IsFixedOffDay
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
			ELSE tblc.ShiftId END
			) AS ShiftId
		,0 AS IsOffDay
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,0 AS IsFixedOffDay
		,1 AS IsPublic
		,@MofifiedDate
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
		AND empsum.IsDefaultSchedule = 0
		INNER JOIN #tblDate AS tblc ON emp.EmpId = tblc.EmpId
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON tblc.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR emp.EmpId = @EmpId
			)
		AND (
			emp.ResignedDate IS NULL
			OR emp.ResignedDate > tblc.WorkDate
			)
		AND emp.JoinedDate <= tblc.WorkDate
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
			WHERE emp.EmpId = empca.EmpId
				AND tblc.WorkDate = empca.WorkDate
			)

	UPDATE empca
	SET empca.ShiftId = tblc.ShiftId
		,empca.ModifiedDate = @MofifiedDate
	FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblDate AS tblc WITH (NOLOCK) ON empca.EmpId = tblc.EmpId
		AND empca.WorkDate = tblc.WorkDate
	WHERE empca.ShiftId IS NULL
		AND tblc.ShiftId IS NOT NULL
		AND empca.IsOffDay = 0
		AND empca.IsHoliday = 0
		AND empca.IsLeaveDay = 0
		AND empca.IsKeepInView = 0
		AND empca.ChangeShiftId IS NULL

	DROP TABLE #tblDate
END
