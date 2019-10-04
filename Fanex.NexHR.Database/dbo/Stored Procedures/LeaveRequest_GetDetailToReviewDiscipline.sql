
CREATE PROCEDURE [dbo].[LeaveRequest_GetDetailToReviewDiscipline] @RequestId BIGINT
AS
/*
Created: 20190426@Quinn.Vo
Task: get leave request detail
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @LeaveType INT
		,@GeneralLeaveType INT = 0
		,@AnnualLeaveType INT = 2
		,@KIVLeaveType INT = 10
		,@UnpaidLeaveType INT = 25
		,@LeaveWorkType TINYINT = 2
		,@TotalDays DECIMAL(5, 1)

	SELECT @LeaveType = LeaveType
		,@TotalDays = TotalDays
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	CREATE TABLE #LeaveDetail (
		WorkDate DATETIME
		,LeaveTypeId INT
		,LeaveTypeName NVARCHAR(200)
		,IsHalfDay BIT
		,FromTime DATETIME
		,ToTime DATETIME
		,IsFromAfterday BIT
		,IsToAfterday BIT
		)

	IF @LeaveType = @GeneralLeaveType
		OR @TotalDays = 0.5
	BEGIN
		INSERT INTO #LeaveDetail (
			WorkDate
			,LeaveTypeId
			,LeaveTypeName
			,IsHalfDay
			,FromTime
			,ToTime
			,IsFromAfterday
			,IsToAfterday
			)
		SELECT ldt.OnDate
			,lt.LeaveTypeId
			,lt.NAME
			,ldt.IsHalfDay
			,ldt.FromTime
			,ldt.ToTime
			,(CASE 
				WHEN CAST(ldt.FromTime AS DATE) > CAST(ldt.OnDate AS DATE) THEN 1
				ELSE 0 END
				) AS IsFromAfterday
			,(CASE 
				WHEN CAST(ldt.ToTime AS DATE) > CAST(ldt.OnDate AS DATE) THEN 1
				ELSE 0 END
				) AS IsToAfterday
		FROM dbo.LeaveRequestDetail AS ldt WITH (NOLOCK)
			LEFT JOIN dbo.LeaveType AS lt WITH (NOLOCK) ON lt.LeaveTypeId = ldt.LeaveType
		WHERE ldt.RequestId = @RequestId
			AND ldt.WorkType = @LeaveWorkType
	END
	ELSE IF @LeaveType = @AnnualLeaveType
		OR @LeaveType = @KIVLeaveType
		OR @LeaveType = @UnpaidLeaveType
	BEGIN
		INSERT INTO #LeaveDetail (
			WorkDate
			,LeaveTypeId
			,LeaveTypeName
			,IsHalfDay
			)
		SELECT ca.WorkDate
			,lt.LeaveTypeId
			,lt.NAME
			,0 AS IsHalfDay
		FROM dbo.LeaveRequest AS lr WITH (NOLOCK)
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON lr.EmpId = ca.EmpId
			AND ca.WorkDate BETWEEN lr.FromDate
				AND lr.ToDate
			LEFT JOIN dbo.LeaveType AS lt WITH (NOLOCK) ON lt.LeaveTypeId = lr.LeaveType
		WHERE lr.RequestId = @RequestId
	END

	SELECT WorkDate
		,IsOffOrHoliday
		,ScheduleStatus
		,IsUnpaidLeaveAbsent
		,IsUnpaidLeave
		,IsAbsent
		,IsUnplannedLeave
		,IsWeekendLeave
	FROM dbo.fnc_GetDisciplineDetailByLeaveRequestId(@RequestId)
	ORDER BY WorkDate

	SELECT WorkDate
		,LeaveTypeId
		,LeaveTypeName
		,IsHalfDay
		,FromTime
		,ToTime
		,IsFromAfterday
		,IsToAfterday
	FROM #LeaveDetail

	DROP TABLE #LeaveDetail
END
