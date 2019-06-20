
CREATE PROCEDURE [dbo].[Dashboard_Get] @EmployeeId INT
	,@Date DATE
AS
/*
Created: Tim.Trinh@2019-06-18
Task: show dashboard info for employee
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @month SMALLINT
		,@fromDate DATE
		,@toDate DATE
		,@LateIn INT
		,@EarlyOut INT
		,@MissIn INT
		,@MissOut INT
		,@TotalPendingOvertime INT
		-- request type
		,@CheckInTypeId TINYINT = 1
		,@CheckOutTypeId TINYINT = 2
		,@LateInTypeId TINYINT = 3
		,@EarlyOutTypeId TINYINT = 4
		,@ChangeShiftTypeId TINYINT = 5
		,@LeaveTypeId TINYINT = 6
		,@KeepInViewTypeId TINYINT = 7
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SET @fromDate = CAST(YEAR(@Date) AS VARCHAR(4)) + '/' + CAST(MONTH(@Date) AS VARCHAR(2)) + '/01'
	SET @toDate = DATEADD(DAY, - 1, DATEADD(MONTH, 1, @fromDate))

	CREATE TABLE #tmpRequest (
		AttendanceDate DATE
		,LateIn BIT
		,EarlyOut BIT
		,RequestType INT
		,RequestId_miss INT
		,RequestId_io INT
		,InFixed DATETIME
		,OutFixed DATETIME
		)

	INSERT INTO #tmpRequest
	SELECT attendance.AttendanceDate
		,attendance.LateIn
		,attendance.EarlyOut
		,r.RequestType
		,iom.RequestId
		,ior.RequestId
		,InFixed
		,OutFixed
	FROM dbo.EmployeeAttendance AS attendance WITH (NOLOCK)
		LEFT JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON attendance.EmpId = r.EmpId
		AND r.CurrentStatus IN (@PendingStatusId, @VerifiedStatusId)
		LEFT JOIN dbo.InoutMissRequest AS iom WITH (NOLOCK) ON iom.RequestId = r.RequestId
		AND iom.OnDate = attendance.AttendanceDate
		LEFT JOIN dbo.InoutRequest AS ior WITH (NOLOCK) ON ior.RequestId = r.RequestId
		AND ior.FromDate = attendance.AttendanceDate
	WHERE attendance.AttendanceDate BETWEEN @fromDate
			AND @todate
		AND attendance.EmpId = @EmployeeId
		AND attendance.ShiftId IS NOT NULL

	DELETE tmp
	FROM #tmpRequest tmp
		INNER JOIN #tmpRequest tmp1 ON tmp1.AttendanceDate = tmp.AttendanceDate
	WHERE (
			tmp1.LateIn = 1
			AND tmp1.RequestType = @LateInTypeId
			)
		OR (
			tmp1.EarlyOut = 1
			AND tmp1.RequestType = @EarlyOutTypeId
			)
		OR (
			tmp1.InFixed IS NULL
			AND tmp1.RequestType = @CheckInTypeId
			)
		OR (
			tmp1.OutFixed IS NULL
			AND tmp1.RequestType = @CheckOutTypeId
			)
		OR (
			tmp1.OutFixed IS NULL
			AND CONVERT(NVARCHAR(20), tmp.AttendanceDate, 111) = CONVERT(NVARCHAR(20), GETDATE(), 111)
			)

	SELECT @LateIn = SUM(CASE 
				WHEN ISNULL(LateIn, 0) > 0 THEN 1
				ELSE 0
				END)
		,@EarlyOut = SUM(CASE 
				WHEN ISNULL(EarlyOut, 0) > 0 THEN 1
				ELSE 0
				END)
		,@MissIn = SUM(CASE 
				WHEN InFixed IS NULL THEN 1
				ELSE 0
				END)
		,@MissOut = SUM(CASE 
				WHEN OutFixed IS NULL
					AND AttendanceDate < @todate THEN 1
				ELSE 0
				END)
	FROM (
		SELECT DISTINCT AttendanceDate
			,LateIn
			,EarlyOut
			,InFixed
			,OutFixed
		FROM #tmpRequest
		) tbl

	SELECT @TotalPendingOvertime = COUNT(1)
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE EmpId = @EmployeeId
		AND CurrentStatus = @PendingStatusId
		AND (
			ISNULL(NormalHours, 0) <> 0
			OR ISNULL(OffHours, 0) <> 0
			OR ISNULL(PublicHours, 0) <> 0
			)

	SELECT @MissIn + @MissOut AS ForgotInOut
		,@LateIn + @EarlyOut AS LateInEarlyOut
		,SUM(CASE 
				WHEN RequestType IN (@CheckInTypeId, @CheckOutTypeId)
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
				ELSE 0
				END) AS TotalCheckInOutRequest
		,SUM(CASE 
				WHEN RequestType IN (@CheckInTypeId, @CheckOutTypeId)
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
				ELSE 0
				END) AS TotalPendingCheckInOutRequest
		,SUM(CASE 
				WHEN RequestType IN (@LateInTypeId, @EarlyOutTypeId)
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
				ELSE 0
				END) AS TotalLateInEarlyOutRequest
		,SUM(CASE 
				WHEN RequestType IN (@LateInTypeId, @EarlyOutTypeId)
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
				ELSE 0
				END) AS TotalPendingLateInEarlyOutRequest
		,SUM(CASE 
				WHEN RequestType = @ChangeShiftTypeId
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
				ELSE 0
				END) AS TotalChangedShiftRequest
		,SUM(CASE 
				WHEN RequestType = @ChangeShiftTypeId
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
				ELSE 0
				END) AS TotalPendingChangedShiftRequest
		,SUM(CASE 
				WHEN RequestType = @LeaveTypeId
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
				ELSE 0
				END) AS TotalLeaveRequest
		,SUM(CASE 
				WHEN RequestType = @LeaveTypeId
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
				ELSE 0
				END) AS TotalPendingLeaveRequest
		,SUM(CASE 
				WHEN RequestType = @KeepInViewTypeId
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
				ELSE 0
				END) AS TotalKIVRequest
		,SUM(CASE 
				WHEN RequestType = @KeepInViewTypeId
					AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
				ELSE 0
				END) AS TotalPendingKIVRequest
		,@TotalPendingOvertime AS TotalPendingOverTimeRequest
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE CreatedDate BETWEEN @fromDate
			AND @todate
		AND EmpId = @EmployeeId

	DROP TABLE #tmpRequest
END