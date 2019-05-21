
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

	IF @FromDate IS NULL
	BEGIN
		SET @FromDate = GETDATE()
	END

	IF @ToDate IS NULL
	BEGIN
		SET @ToDate = DATEADD(DAY, 6, @FromDate)
	END

	CREATE TABLE #tblCalendar (WorkDate DATE)

	WHILE @FromDate <= @ToDate
	BEGIN
		INSERT INTO #tblCalendar (WorkDate)
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
	SELECT DISTINCT e.EmpId
		,tblc.WorkDate
		,(CASE 
			WHEN (
					es.IsLocal = 1
					OR (
						es.IsLocal = 0
						AND es.IsExpatUseHoliday = 1
						)
					)
				AND hld.Id IS NOT NULL THEN NULL
			ELSE shift.ShiftId END
			) AS ShiftId
		,0 AS IsOffDay
		,(CASE 
			WHEN (
					es.IsLocal = 1
					OR (
						es.IsLocal = 0
						AND es.IsExpatUseHoliday = 1
						)
					)
				AND hld.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,1
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON es.EmpId = e.EmpId
		AND es.IsDefaultSchedule = 0
		,#tblCalendar AS tblc
		CROSS APPLY (
		SELECT TOP 1 shift.ShiftId
		FROM dbo.ScheduleArrangementDefaultShift AS shift WITH (NOLOCK)
		WHERE shift.EmployeeId = e.EmpId
			AND shift.ShiftId > 0
			AND shift.FromDate <= @FromDate
		ORDER BY shift.FromDate DESC
		) AS shift
		LEFT JOIN dbo.Holiday AS hld WITH (NOLOCK) ON tblc.WorkDate BETWEEN hld.FromDate
			AND hld.ToDate
	WHERE e.[Enabled] = 1
		AND ISNULL(e.IsDeleted, 0) = 0
		AND (
			@EmployeeId IS NULL
			OR e.EmpId = @EmployeeId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
			WHERE e.EmpId = ec.EmpId
				AND tblc.WorkDate = ec.WorkDate
			)

	EXEC dbo.ScheduleArrangement_UpdateOffDayReplacement @ActorId = - 1
		,@FromDate = @FromDate
		,@ToDate = @ToDate
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Insert default shift unsuccessfully!'
	END

	DROP TABLE #tblCalendar
END
