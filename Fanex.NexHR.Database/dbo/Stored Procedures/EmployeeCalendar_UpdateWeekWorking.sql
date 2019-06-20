
CREATE PROCEDURE [dbo].[EmployeeCalendar_UpdateWeekWorking] @empId INT
	,@date DATETIME
	,@monShift INT
	,@tueShift INT
	,@wedShift INT
	,@thuShift INT
	,@friShift INT
	,@satShift INT
	,@sunShift INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-29
Task: Update working calendar for an employee in a week
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	@Id: employee ID
	@date: the date to identify the week
	@monShift: shift id for Monday
	@tueShift: shift id for Tueday
	@wedShift: shift id for Wedday
	@thuShift: shift id for Thursday
	@friShift: shift id for Friday
	@satShift: shift id for Saturday
	@sunShift: shift id for Sunday

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME
		,@todate DATETIME
		,@dw INT
		,@isHoliday BIT
		,@isUseHoliday BIT

	SET @fromdate = DATEADD(DD, 1 - DATEPART(DW, @date), @Date)

	SELECT @todate = DATEADD(d, 6, @fromdate)
		,@isUseHoliday = 0

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeSummary AS es WITH (NOLOCK)
				WHERE es.EmpId = @EmpId
					AND (
						es.IsLocal = 1
						OR (
							es.IsLocal = 0
							AND es.IsExpatUseHoliday = 1
							)
						)
					AND EXISTS (
						SELECT 1
						FROM dbo.Holiday WITH (NOLOCK)
						WHERE @date BETWEEN FromDate
								AND ToDate
						)
				)
			)
	BEGIN
		SET @isUseHoliday = 1
	END

	UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	SET @dw = datepart(dw, WorkDate)
		,@isHoliday = CASE 
			WHEN @isUseHoliday = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.Holiday WITH (NOLOCK)
					WHERE WorkDate BETWEEN FromDate
							AND ToDate
					) THEN 1
			ELSE 0
			END
		,IsHoliday = @isHoliday
		,ShiftId = CASE 
			WHEN @isHoliday = 1 THEN NULL
			ELSE (CASE 
					WHEN @dw = 1 THEN @SunShift
					WHEN @dw = 2 THEN @MonShift
					WHEN @dw = 3 THEN @TueShift
					WHEN @dw = 4 THEN @WedShift
					WHEN @dw = 5 THEN @ThuShift
					WHEN @dw = 6 THEN @FriShift
					WHEN @dw = 7 THEN @SatShift END
					)
			END
		,isOffDay = CASE 
			WHEN @dw = 1
				AND @SunShift IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			WHEN @dw = 2
				AND @MonShift IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			WHEN @dw = 3
				AND @TueShift IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			WHEN @dw = 4
				AND @WedShift IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			WHEN @dw = 5
				AND @ThuShift IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			WHEN @dw = 6
				AND @FriShift IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			WHEN @dw = 7
				AND @SatShift IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			ELSE 0
			END
		,IsLeaveDay = CASE 
			WHEN @dw = 1
				AND @SunShift IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			WHEN @dw = 2
				AND @MonShift IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			WHEN @dw = 3
				AND @TueShift IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			WHEN @dw = 4
				AND @WedShift IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			WHEN @dw = 5
				AND @ThuShift IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			WHEN @dw = 6
				AND @FriShift IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			WHEN @dw = 7
				AND @SatShift IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			ELSE 0
			END
		,ModifiedDate = GETDATE()
	WHERE WorkDate BETWEEN @fromdate
			AND @todate
		AND empId = @empId

	-- update attendance record 
	EXEC EmployeeAttendance_ProcessingAndInsert @empId = @empId
		,@fromTime = @fromdate
		,@toTime = @todate
		,@onlyUpdate = 1

	IF @ErrCode <> 0
		SELECT @ErrCode = 100
			,@ErrMsg = N'working calendar for an employee in a week failed.'
	ELSE
		SELECT @ErrCode = 0
			,@ErrCode = ''
END