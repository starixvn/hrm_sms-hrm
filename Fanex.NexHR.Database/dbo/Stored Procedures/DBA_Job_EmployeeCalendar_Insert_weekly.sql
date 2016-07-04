
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[DBA_Job_EmployeeCalendar_Insert_weekly] @empid INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @fromdate DATETIME
		,@todate DATETIME

	SELECT @fromdate = GETDATE()
		,@todate = DATEADD(DD, 120, GETDATE())

	SET @fromdate = convert(VARCHAR(10), @fromdate, 111)
	SET @todate = convert(VARCHAR(10), @todate, 111)

	DECLARE @tblCalendare TABLE (
		WorkDate SMALLDATETIME
		,IsEnabled BIT
		)

	WHILE @fromdate <= @todate
	BEGIN
		INSERT INTO @tblCalendare (
			WorkDate
			,IsEnabled
			)
		VALUES (
			@fromdate
			,1
			)

		SET @fromdate = dateadd(d, 1, @fromdate)
	END

	-- insert vao working calendar
	INSERT INTO dbo.EmployeeCalendar (
		EmpID
		,WorkDate
		,ShiftID
		,isOffDay
		,IsHoliday
		,IsPublic
		)
	SELECT DISTINCT s.empid
		,tblCalendare.WorkDate AS WorkDate
		,CASE 
			WHEN datepart(dw, tblCalendare.WorkDate) = 1 THEN SunShift
			WHEN datepart(dw, tblCalendare.WorkDate) = 2 THEN MonShift
			WHEN datepart(dw, tblCalendare.WorkDate) = 3 THEN TueShift
			WHEN datepart(dw, tblCalendare.WorkDate) = 4 THEN WedShift
			WHEN datepart(dw, tblCalendare.WorkDate) = 5 THEN ThuShift
			WHEN datepart(dw, tblCalendare.WorkDate) = 6 THEN FriShift
			WHEN datepart(dw, tblCalendare.WorkDate) = 7 THEN SatShift
			END AS ShiftID
		,CASE 
			WHEN es.IsLocal = 1
				AND hld.Id IS NOT NULL THEN 0
			WHEN datepart(dw, tblCalendare.WorkDate) = 1
				AND SunShift IS NULL THEN 1
			WHEN datepart(dw, tblCalendare.WorkDate) = 2
				AND MonShift IS NULL THEN 1
			WHEN datepart(dw, tblCalendare.WorkDate) = 3
				AND TueShift IS NULL THEN 1
			WHEN datepart(dw, tblCalendare.WorkDate) = 4
				AND WedShift IS NULL THEN 1
			WHEN datepart(dw, tblCalendare.WorkDate) = 5
				AND ThuShift IS NULL THEN 1
			WHEN datepart(dw, tblCalendare.WorkDate) = 6
				AND FriShift IS NULL THEN 1
			WHEN datepart(dw, tblCalendare.WorkDate) = 7
				AND SatShift IS NULL THEN 1
			ELSE 0
			END AS isOffDay
		,CASE 
			WHEN es.IsLocal = 1
				AND hld.Id IS NOT NULL THEN 1
			ELSE 0
			END AS isHoliday
		,1
	FROM dbo.EmployeeDefaultShift s WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.empid = s.empid
		INNER JOIN @tblCalendare tblCalendare ON tblCalendare.IsEnabled = e.Enabled
		INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON es.empid = s.empid
		LEFT JOIN EmployeeCalendar c ON e.EmpId = c.EmpId
		AND c.WorkDate = tblCalendare.WorkDate
		LEFT JOIN dbo.Holiday hld WITH (NOLOCK) ON tblCalendare.WorkDate BETWEEN hld.FromDate
			AND hld.ToDate
	WHERE es.IsLocal = 1
		AND es.IsDefaultSchedule = 1
		AND isnull(IsDeleted, 0) = 0
		AND (
			@empid IS NULL
			OR s.empid = @empid
			)
		AND c.EmpId IS NULL
		AND (
			s.SunShift IS NOT NULL
			OR s.MonShift IS NOT NULL
			OR s.TueShift IS NOT NULL
			OR s.WedShift IS NOT NULL
			OR s.ThuShift IS NOT NULL
			OR s.FriShift IS NOT NULL
			OR s.SatShift IS NOT NULL
			)

	IF @@ERROR <> 0
		SELECT @ErrCode = 100
			,@ErrMsg = N'Create Default Calendar for employee failed'
	ELSE
		SELECT @ErrCode = 0
			,@ErrMsg = ''
END