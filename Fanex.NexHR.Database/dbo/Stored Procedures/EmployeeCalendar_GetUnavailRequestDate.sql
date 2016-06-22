---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetUnavailRequestDate] 
	@EmpId int,
	@fromDate datetime,
	@toDate datetime
AS
/*
Created: Tom.Vo@2013-10-29
Task: get list days which is not Work
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @MaxDate datetime, @MinDate datetime,@fromDate_LC datetime,@toDate_LC datetime

	CREATE TABLE #tmpCalendar(WorkDate DATETIME)

	SELECT @fromDate=CONVERT(NVARCHAR(20),@fromDate,111),@toDate=CONVERT(NVARCHAR(20),@toDate,111)

	SELECT  @fromDate_LC = @fromDate  , @toDate_LC = @toDate
	WHILE (@fromDate_LC<=@toDate_LC)
		BEGIN
			INSERT INTO #tmpCalendar
			VALUES (@fromDate_LC)

			SET @fromDate_LC = DATEADD(DD,1,@fromDate_LC)
		END

	DELETE #tmpCalendar
	WHERE WorkDate IN (
		SELECT WorkDate 
		FROM dbo.EmployeeCalendar WITH(NOLOCK)
		WHERE (WorkDate BETWEEN @fromDate AND @toDate)
			AND EmpID= @EmpId 
			AND NOT (IsOffDay = 1 OR IsHoliday = 1 OR (IsLeaveDay = 1 AND LeaveDay = 1) OR (IsOffDay = 0 AND IsHoliday = 0 AND IsLeaveDay = 0 AND ShiftId IS NULL))
			-- AND NOT (((IsHoliday = 1 OR IsOffDay = 1) AND ShiftId IS NULL) OR (IsLeaveDay = 1 and LeaveDay=1))
		)
	
	SELECT WorkDate 
	FROM #tmpCalendar

	DROP TABLE #tmpCalendar
END
