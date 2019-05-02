
CREATE PROCEDURE [dbo].[DBA_Job_EmployeeCalendar_Insert_weekly] @EmpId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @fromdate DATE = GETDATE()
		,@todate DATE = DATEADD(DD, 120, GETDATE())

	CREATE TABLE #tblCalendar (WorkDate DATE)

	WHILE @fromdate <= @todate
	BEGIN
		INSERT INTO #tblCalendar (WorkDate)
		VALUES (@fromdate)

		SET @fromdate = DATEADD(d, 1, @fromdate)
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
			WHEN datepart(dw, tblc.WorkDate) = 1 THEN SunShift
			WHEN datepart(dw, tblc.WorkDate) = 2 THEN MonShift
			WHEN datepart(dw, tblc.WorkDate) = 3 THEN TueShift
			WHEN datepart(dw, tblc.WorkDate) = 4 THEN WedShift
			WHEN datepart(dw, tblc.WorkDate) = 5 THEN ThuShift
			WHEN datepart(dw, tblc.WorkDate) = 6 THEN FriShift
			WHEN datepart(dw, tblc.WorkDate) = 7 THEN SatShift END
			) AS ShiftId
		,(CASE 
			WHEN (
					es.IsLocal = 1
					OR (
						es.IsLocal = 0
						AND es.IsExpatUseHoliday = 1
						)
					)
				AND hld.Id IS NOT NULL THEN 0
			WHEN datepart(dw, tblc.WorkDate) = 1
				AND SunShift IS NULL THEN 1
			WHEN datepart(dw, tblc.WorkDate) = 2
				AND MonShift IS NULL THEN 1
			WHEN datepart(dw, tblc.WorkDate) = 3
				AND TueShift IS NULL THEN 1
			WHEN datepart(dw, tblc.WorkDate) = 4
				AND WedShift IS NULL THEN 1
			WHEN datepart(dw, tblc.WorkDate) = 5
				AND ThuShift IS NULL THEN 1
			WHEN datepart(dw, tblc.WorkDate) = 6
				AND FriShift IS NULL THEN 1
			WHEN datepart(dw, tblc.WorkDate) = 7
				AND SatShift IS NULL THEN 1
			ELSE 0 END
			) AS IsOffDay
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
	FROM dbo.EmployeeDefaultShift AS s WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = s.EmpID
		AND e.[Enabled] = 1
		AND ISNULL(e.IsDeleted, 0) = 0
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON es.EmpId = e.EmpId
		AND es.IsDefaultSchedule = 1
		,#tblCalendar AS tblc
		LEFT JOIN dbo.Holiday AS hld WITH (NOLOCK) ON tblc.WorkDate BETWEEN hld.FromDate
			AND hld.ToDate
	WHERE (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND (
			s.SunShift IS NOT NULL
			OR s.MonShift IS NOT NULL
			OR s.TueShift IS NOT NULL
			OR s.WedShift IS NOT NULL
			OR s.ThuShift IS NOT NULL
			OR s.FriShift IS NOT NULL
			OR s.SatShift IS NOT NULL
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
			WHERE e.EmpId = ec.EmpId
				AND tblc.WorkDate = ec.WorkDate
			)

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Create Default Calendar for employee failed'
	END

	DROP TABLE #tblCalendar
END