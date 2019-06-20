
CREATE PROCEDURE [dbo].[LeaveRequest_InsertKIVLeaveTrans] @RequestId BIGINT
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

	DECLARE @KIVLeaveDay NUMERIC(18, 2)
		,@KIVLeaveHour INT
		,@KeepInViewByDayLeaveType INT = 10
		,@KeepInViewByHourLeaveType INT = 38

	IF EXISTS (
			SELECT 1
			FROM dbo.KeepInViewByDayTrans WITH (NOLOCK)
			WHERE Requestid = @RequestId
			)
	BEGIN
		DELETE
		FROM dbo.KeepInViewByDayTrans
		WHERE Requestid = @RequestId
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.KeepInViewByHourTrans WITH (NOLOCK)
			WHERE Requestid = @RequestId
			)
	BEGIN
		DELETE
		FROM dbo.KeepInViewByHourTrans
		WHERE RequestId = @RequestId
	END
	
	IF EXISTS (
			SELECT 1
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @RequestId
			)
	BEGIN
		SELECT @KIVLeaveDay = SUM(
			CASE WHEN LeaveType = @KeepInViewByDayLeaveType
				THEN
					CASE 
					WHEN IsHalfDay = 1 THEN 0.5
					ELSE 1
					END
					ELSE
					0
					END
					)
			,@KIVLeaveHour = SUM(CASE WHEN LeaveType = @KeepInViewByHourLeaveType
				THEN
					DATEDIFF(MI, FromTime, ToTime)
					ELSE
					0
					END)
		FROM dbo.LeaveRequestDetail WITH (NOLOCK)
		WHERE RequestId = @RequestId
	END
	ELSE IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveRequest WITH (NOLOCK)
				WHERE RequestId = @RequestId
					AND LeaveType = @KeepInViewByDayLeaveType
				)
			)
	BEGIN
		SELECT @KIVLeaveDay = TotalDays
		FROM dbo.LeaveRequest WITH (NOLOCK)
		WHERE RequestId = @RequestId
			AND LeaveType = @KeepInViewByDayLeaveType
	END

	IF (ISNULL(@KIVLeaveDay, 0) > 0)
	BEGIN
		INSERT INTO dbo.KeepInViewByDayTrans (
			[EmpId]
			,[RequestId]
			,[CreditDays]
			,[DeductDays]
			,[InYear]
			,[InsertedDate]
			)
		SELECT EmpId
			,@RequestId
			,0
			,ISNULL(@KIVLeaveDay, 0)
			,YEAR(ToDate)
			,GETDATE()
		FROM dbo.LeaveRequest lr WITH (NOLOCK)
		WHERE Requestid = @RequestId
	END

	IF (ISNULL(@KIVLeaveHour, 0) > 0)
	BEGIN
		INSERT INTO dbo.KeepInViewByHourTrans (
			EmpId
			,WorkDate
			,RequestId
			,CreditMinutes
			,DeductMinutes
			,InsertedDate
			)
		SELECT EmpId
			,ldt.OnDate
			,@RequestId
			,0
			,DATEDIFF(MI, ldt.FromTime, ldt.ToTime)
			,GETDATE()
		FROM dbo.LeaveRequestDetail ldt WITH (NOLOCK)
		WHERE Requestid = @RequestId
	END
END