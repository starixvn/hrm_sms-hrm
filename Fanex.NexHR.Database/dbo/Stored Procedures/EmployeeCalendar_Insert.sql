
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
	IF (
			EXISTS (
				SELECT 1
				FROM dbo.Holiday WITH (NOLOCK)
				WHERE @fromdate BETWEEN FromDate
						AND ToDate
				)
			)
		SET @isHoliday = 1

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
		,CASE 
			WHEN (
					es.IsLocal = 1
					OR (
						es.IsLocal = 0
						AND es.IsExpatUseHoliday = 1
						)
					)
				AND @isHoliday = 1 THEN NULL
			ELSE CASE 
					WHEN @dw = 1 THEN SunShift
					WHEN @dw = 2 THEN MonShift
					WHEN @dw = 3 THEN TueShift
					WHEN @dw = 4 THEN WedShift
					WHEN @dw = 5 THEN ThuShift
					WHEN @dw = 6 THEN FriShift
					WHEN @dw = 7 THEN SatShift
					END
			END AS ShiftID
		,CASE 
			WHEN (
					es.IsLocal = 1
					OR (
						es.IsLocal = 0
						AND es.IsExpatUseHoliday = 1
						)
					)
				AND @isHoliday = 1 THEN 0
			ELSE CASE 
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
					ELSE 0
					END
			END AS isOffDay
		,CASE 
			WHEN (
					es.IsLocal = 1
					OR (
						es.IsLocal = 0
						AND es.IsExpatUseHoliday = 1
						)
					)
				AND @isHoliday = 1 THEN 1
			ELSE 0
			END AS isHoliday
		,GETDATE()
	FROM dbo.EmployeeDefaultShift s WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.empid = s.empid
		INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON es.empid = s.empid
	WHERE isnull(IsDeleted, 0) = 0
		AND (
			@empid IS NULL
			OR s.empid = @empid
			)
		AND es.IsDefaultSchedule = 1
		AND isnull(ResignedDate, '2079/01/01') > @fromdate
		AND NOT EXISTS (
			SELECT TOP 1 1
			FROM EmployeeCalendar c WITH (NOLOCK)
			WHERE WorkDate = @fromdate
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