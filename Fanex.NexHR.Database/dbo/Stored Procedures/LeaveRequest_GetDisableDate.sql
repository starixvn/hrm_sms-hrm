
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

	SELECT tmpca.RequestId
		,tmpca.WorkDate
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
	FROM #tblEmployeeCalendar AS empca WITH (NOLOCK)
	WHERE (
			empca.IsLeaveDay = 1
			AND empca.LeaveDay = 1
			)
		OR empca.IsLeaveOffday = 1
		OR empca.IsOffDay = 1
		OR empca.IsHoliday = 1
		OR empca.IsOvertime = 1
		OR EXISTS (
			SELECT lr.RequestId
			FROM dbo.LeaveRequest AS lr WITH (NOLOCK)
			WHERE lr.RequestId = empca.RequestId
				AND (
					@RequestId IS NULL
					OR lr.RequestId <> @requestId
					)
				AND empca.WorkDate < lr.FromDate
				AND empca.WorkDate > lr.ToDate
			)

	DROP TABLE #tblEmployeeCalendar
END
