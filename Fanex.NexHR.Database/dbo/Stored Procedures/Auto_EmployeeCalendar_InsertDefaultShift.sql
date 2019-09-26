
CREATE PROCEDURE [dbo].[Auto_EmployeeCalendar_InsertDefaultShift] @EmployeeId INT = NULL
	,@FromDate DATE = NULL
	,@ToDate DATE = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-05-20
Task: insert default shift
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	
	DECLARE @tempDate DATE

	IF @FromDate IS NULL
	BEGIN
		SET @FromDate = GETDATE()
	END

	IF @ToDate IS NULL
	BEGIN
		SET @ToDate = DATEADD(DAY, 6, @FromDate)
	END
	
	SET @tempDate = @FromDate

	CREATE TABLE #tblDate (WorkDate DATE)

	WHILE @tempDate <= @ToDate
	BEGIN
		INSERT INTO #tblDate (WorkDate)
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
		)
	SELECT DISTINCT emp.EmpId
		,tblc.WorkDate
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN NULL
			ELSE shift.ShiftId END
			) AS ShiftId
		,0 AS IsOffDay
		,0 AS IsFixedOffDay
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR empsum.IsExpatUseHoliday = 1
					)
				AND holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,1
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
		AND empsum.IsDefaultSchedule = 0
		OUTER APPLY (
		SELECT TOP 1 shift.ShiftId
		FROM dbo.ScheduleArrangementDefaultShift AS shift WITH (NOLOCK)
		WHERE shift.EmployeeId = emp.EmpId
			AND shift.ShiftId > 0
			AND shift.FromDate <= @FromDate
		ORDER BY shift.FromDate DESC
		) AS shift
		,#tblDate AS tblc
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON tblc.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND (
			@EmployeeId IS NULL
			OR emp.EmpId = @EmployeeId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
			WHERE emp.EmpId = ec.EmpId
				AND tblc.WorkDate = ec.WorkDate
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
			,@ErrMsg = N'Insert default shift unsuccessfully!'
	END

	DROP TABLE #tblDate
END
