
CREATE PROCEDURE [dbo].[LeaveRequest_GetDisableDate] @empId INT
	,@fromDate DATE
	,@toDate DATE
	,@requestId BIGINT = NULL
AS
/*
Created: Tim.Trinh@2019-10-15
Task: get disable date for request
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE
		-- request status
		@NotVerifiedStatusId TINYINT = 3
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
		,tmpca.IsLeaveOffday
		,tmpca.IsLeaveDay
		,tmpca.LeaveDay
	INTO #tblEmployeeCalendar
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.tmpEmployeeCalendar AS tmpca WITH (NOLOCK) ON emp.EmpId = tmpca.EmpId
		AND tmpca.WorkDate BETWEEN @fromDate
			AND @toDate
	WHERE emp.EmpId = @EmpId

	SELECT WorkDate
	FROM #tblEmployeeCalendar
	WHERE (
			IsLeaveDay = 1
			AND LeaveDay = 1
			)
		OR IsLeaveOffday = 1
		OR IsOffDay = 1
		OR IsHoliday = 1
		OR IsOvertime = 1
		OR EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
				AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
			WHERE (
					@RequestId IS NULL
					OR r.RequestId <> @requestId
					)
				AND WorkDate < lr.FromDate
				AND WorkDate > lr.ToDate
			)

	DROP TABLE #tblEmployeeCalendar
END
