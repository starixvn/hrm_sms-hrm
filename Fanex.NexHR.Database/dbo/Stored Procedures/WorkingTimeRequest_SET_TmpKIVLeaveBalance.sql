
CREATE PROCEDURE [dbo].[WorkingTimeRequest_SET_TmpKIVLeaveBalance] 
	@RequestId BIGINT
	,@EmpID INT
	,@TotalDays DECIMAL(18, 2)
AS
/*
Created: Quinn.Vo@20150927
Task: insert kiv temp leave balance.
DB: HRM
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @KIVLeave NUMERIC(18, 2)
	DECLARE @KIVLeaveCode VARCHAR(10)
	SELECT @KIVLeaveCode = 'KIV'

	IF EXISTS (
			SELECT 1
			FROM dbo.[tmpKIVLeaveBalance] WITH (NOLOCK)
			WHERE Requestid = @Requestid
			)
	BEGIN
		SELECT @KIVLeave = SUM(TotalDays)
		FROM dbo.tmpKIVLeaveBalance WITH (NOLOCK)
		WHERE Requestid = @Requestid
			AND [Status] = 1

		IF ISNULL(@KIVLeave, 0) > 0
		BEGIN
			UPDATE EmployeeSummary WITH (ROWLOCK, UPDLOCK)
			SET KeepInViewBalance = ISNULL(KeepInViewBalance, 0) + ISNULL(@KIVLeave, 0)
			WHERE EmpId = @EmpId
		END

		DELETE
		FROM dbo.[tmpKIVLeaveBalance]
		WHERE Requestid = @Requestid
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @Requestid
			)
	BEGIN
		SELECT @KIVLeave = SUM(CASE 
					WHEN cl.IsHalfDay = 1 THEN 0.5
					ELSE 1
					END)
		FROM dbo.LeaveRequestDetail cl WITH (NOLOCK)
			INNER JOIN dbo.LeaveType l WITH (NOLOCK) ON cl.LeaveType = l.LeaveTypeId
		WHERE l.Code = @KIVLeaveCode
		AND cl.EmpId = @EmpID
		AND cl.RequestId = @RequestId
	END
	ELSE IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveRequest AS l WITH (NOLOCK)
					INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON l.LeaveType = lv.LeaveTypeId
				WHERE lv.Code = @KIVLeaveCode
					AND l.RequestId = @Requestid
				)
			)
	BEGIN
		SELECT @KIVLeave = l.TotalDays
		FROM dbo.LeaveRequest AS l WITH (NOLOCK)
			INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON l.LeaveType = lv.LeaveTypeId
		WHERE lv.Code = @KIVLeaveCode
			AND l.RequestId = @Requestid
	END

	IF (ISNULL(@KIVLeave, 0) > 0)
	BEGIN
		INSERT INTO dbo.[tmpKIVLeaveBalance] (
			EmpID
			,Requestid
			,TotalDays
			,InYear
			,[Status]
			)
		SELECT TOP 1 @EmpID
			,@Requestid
			,ISNULL(@KIVLeave, 0)
			,YEAR(TODATE)
			,0
		FROM dbo.LeaveRequest lr WITH (NOLOCK)
		WHERE Requestid = @Requestid

		UPDATE EmployeeSummary WITH (ROWLOCK, UPDLOCK)
		SET KeepInViewBalance = ISNULL(KeepInViewBalance, 0) - ISNULL(@KIVLeave, 0)
		WHERE EmpId = @EmpId
	END
END
