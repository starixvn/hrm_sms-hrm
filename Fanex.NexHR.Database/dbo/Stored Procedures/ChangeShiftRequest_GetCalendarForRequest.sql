
CREATE PROCEDURE [dbo].[ChangeShiftRequest_GetCalendarForRequest] @empId INT
	,@typeId TINYINT
	,@fromDate DATE
	,@toDate DATE
	,@requestId BIGINT = NULL
AS
/*
Created: Tim.Trinh@2019-09-19
Task: get calendar for request
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE
		-- change shift type
		@ChangeShift TINYINT = 1
		,@MoveOffDay TINYINT = 2

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @fromDate
		,@toDate = @toDate
		,@empIds = @empId

	SELECT tmpca.WorkDate
		,tmpca.ShiftId
		,tmpca.IsOvertime
		,tmpca.ChangeShiftTypeId
		,tmpca.IsKeepInView
		,tmpca.IsHoliday
		,tmpca.IsOffDay
		,tmpca.IsLeaveOffday
		,tmpca.IsLeaveDay
		,tmpca.LeaveDay
	INTO #tblEmployeeCalendar
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.tmpEmployeeCalendar AS tmpca WITH (NOLOCK) ON emp.EmpId = tmpca.EmpId
		AND tmpca.WorkDate BETWEEN @fromDate
			AND @toDate
	WHERE emp.EmpId = @EmpId
		AND (
			@requestId IS NULL
			OR ISNULL(tmpca.RequestId, 0) <> @requestId
			)

	IF @TypeId = @ChangeShift
	BEGIN
		SELECT WorkDate
			,IsOffDay
			,tbl.ShiftId
			,shift.[Name]
		FROM #tblEmployeeCalendar AS tbl
			LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON tbl.ShiftId = shift.ShiftId
		WHERE ChangeShiftTypeId IS NULL
			AND IsKeepInView = 0
			AND IsOvertime = 0
			AND IsHoliday = 0
			AND IsLeaveOffday = 0
			AND (
				IsLeaveDay = 0
				OR LeaveDay = 0.5
				)
	END
	ELSE
	BEGIN
		SELECT WorkDate
			,IsOffDay
			,tbl.ShiftId
			,shift.[Name]
		FROM #tblEmployeeCalendar AS tbl
			LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON tbl.ShiftId = shift.ShiftId
		WHERE ChangeShiftTypeId IS NULL
			AND IsKeepInView = 0
			AND IsLeaveDay = 0
			AND IsHoliday = 0
			AND IsOvertime = 0
			AND IsLeaveOffday = 0
			AND IsOffDay = 1
	END

	DROP TABLE #tblEmployeeCalendar
END
