CREATE PROCEDURE [dbo].[EmployeeCalendar_Insert] @empid INT
	,@fromdate DATETIME
	,@todate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-21
Task: Insert default working calendar for employee
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON
	-- cut off time
	SET @fromdate = convert(VARCHAR(10), @fromdate, 111)
	SET @todate = convert(VARCHAR(10), @todate, 111)

	DECLARE @dw INT
		,@isHoliday BIT

	WHILE @fromdate <= @todate
	BEGIN
		SET @isHoliday = 0
		SET @dw = datepart(dw, @fromdate)

		--print @dw
		-- kiem tra @fromdate co phai la ngay nghi
		IF EXISTS (
				SELECT 1
				FROM dbo.Holiday WITH (NOLOCK)
				WHERE @fromdate BETWEEN FromDate
						AND ToDate
				)
			SET @isHoliday = 1

		-- se xu ly leave request o day neu can
		-- insert vao working calendar
		INSERT INTO dbo.EmployeeCalendar (
			EmpID
			,WorkDate
			,ShiftID
			,isOffDay
			,IsHoliday
			,ModifiedDate
			)
		SELECT s.empid
			,@fromdate AS WorkDate
			,(CASE 
				WHEN @dw = 1 THEN SunShift
				WHEN @dw = 2 THEN MonShift
				WHEN @dw = 3 THEN TueShift
				WHEN @dw = 4 THEN WedShift
				WHEN @dw = 5 THEN ThuShift
				WHEN @dw = 6 THEN FriShift
				WHEN @dw = 7 THEN SatShift END
				) AS ShiftID
			,(CASE 
				WHEN es.IsLocal = 1
					AND @isHoliday = 1 THEN 0
				WHEN @dw = 1
					AND SunShift IS NULL THEN 1
				WHEN @dw = 2
					AND MonShift IS NULL THEN 1
				WHEN @dw = 3
					AND TueShift IS NULL THEN 1
				WHEN @dw = 4
					AND WedShift IS NULL THEN 1
				WHEN @dw = 5
					AND ThuShift IS NULL THEN 1
				WHEN @dw = 6
					AND FriShift IS NULL THEN 1
				WHEN @dw = 7
					AND SatShift IS NULL THEN 1
				ELSE 0 END
				) AS isOffDay
			,(CASE 
				WHEN es.IsLocal = 1
					AND @isHoliday = 1 THEN 1
				ELSE 0 END
				) AS isHoliday
			,GETDATE()
		FROM dbo.EmployeeDefaultShift AS s WITH (NOLOCK)
			JOIN dbo.Employee AS e WITH (NOLOCK) ON e.empid = s.empid
			LEFT JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON es.empid = s.empid
		WHERE isnull(e.IsDeleted, 0) = 0
			AND (
				@empid IS NULL
				OR s.empid = @empid
				)
			AND es.IsDefaultSchedule = 1
			AND isnull(e.ResignedDate, '2079/01/01') > @fromdate
			AND NOT EXISTS (
				SELECT 1
				FROM EmployeeCalendar AS c WITH (NOLOCK)
				WHERE c.WorkDate = @fromdate
					AND s.empid = c.empid
				)

		SET @fromdate = dateadd(d, 1, @fromdate)
	END

	IF @@ERROR <> 0
		SELECT @ErrCode = 100
			,@ErrMsg = N'Create Default Calendar for employee failed'
	ELSE
		SELECT @ErrCode = 0
			,@ErrMsg = ''
END