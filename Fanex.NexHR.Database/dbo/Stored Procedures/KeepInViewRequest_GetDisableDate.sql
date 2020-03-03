
CREATE PROCEDURE [dbo].[KeepInViewRequest_GetDisableDate] @empId INT
	,@fromDate DATE
	,@toDate DATE
	,@requestId BIGINT = NULL
AS
/*
Created: Tim.Trinh@2019-10-16
Task: get disable date for request
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RequestTypeId TINYINT
		-- request status
		,@NotVerifiedStatusId TINYINT = 3
		,@RejectedStatusId TINYINT = 5
		,@VoidStatusId TINYINT = 7

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @fromDate
		,@toDate = @toDate
		,@empIds = @empId

	SELECT tmpca.WorkDate
		,tmpca.ShiftId
		,tmpca.IsHoliday
		,tmpca.IsOffDay
		,tmpca.IsOvertime
		,tmpca.IsLeaveOffDay
		,tmpca.IsLeaveDay
		,tmpca.LeaveDay
		,tmpca.ChangeShiftTypeId
	INTO #tblEmployeeCalendar
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.tmpEmployeeCalendar AS tmpca WITH (NOLOCK) ON emp.EmpId = tmpca.EmpId
		AND tmpca.WorkDate BETWEEN @fromDate
			AND @toDate
	WHERE emp.EmpId = @EmpId

	SELECT WorkDate
	FROM #tblEmployeeCalendar
	WHERE IsOvertime = 1
		OR (
			IsLeaveDay = 1
			AND LeaveDay = 1
			)
		OR IsLeaveOffDay = 1
		OR ChangeShiftTypeId IS NOT NULL
		OR EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON r.RequestId = kiv.RequestId
				AND WorkDate = kiv.OnDate
				AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
			WHERE @RequestId IS NULL
				OR r.RequestId <> @requestId
			)

	DROP TABLE #tblEmployeeCalendar
END
