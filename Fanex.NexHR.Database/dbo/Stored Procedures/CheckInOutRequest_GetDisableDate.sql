
CREATE PROCEDURE [dbo].[CheckInOutRequest_GetDisableDate] @empId INT
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

	DECLARE @RequestTypeId TINYINT
		-- request status
		,@NotVerifiedStatusId TINYINT = 3
		,@RejectedStatusId TINYINT = 5
		,@VoidStatusId TINYINT = 7

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @fromDate
		,@toDate = @toDate
		,@empIds = @empId

	IF @RequestId IS NOT NULL
	BEGIN
		SELECT @RequestTypeId = r.RequestType
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.CheckInOutRequest AS cio WITH (NOLOCK) ON r.RequestId = cio.RequestId
			AND cio.RequestId = @RequestId
	END

	SELECT tmpca.WorkDate
		,tmpca.ShiftId
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

	SELECT WorkDate
	FROM #tblEmployeeCalendar
	WHERE ShiftId IS NULL
		OR (
			IsLeaveDay = 1
			AND LeaveDay = 1
			)
		OR IsLeaveOffday = 1
		OR (
			IsOffDay = 1
			AND ShiftId IS NULL
			)
		OR (
			IsHoliday = 1
			AND ShiftId IS NULL
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.CheckInOutRequest AS cio WITH (NOLOCK) ON r.RequestId = cio.RequestId
				AND WorkDate = cio.OnDate
				AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
			WHERE @RequestId IS NULL
				OR (
					r.RequestId <> @requestId
					AND r.RequestType = @RequestTypeId
					)
			)

	DROP TABLE #tblEmployeeCalendar
END
