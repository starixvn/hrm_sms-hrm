
CREATE PROCEDURE [dbo].[LeaveRequest_InsertTmpKIVLeaveTrans] @RequestId BIGINT
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

	DECLARE @KIVLeaveDay NUMERIC(18, 2)
		,@KeepInViewLeaveType INT = 10

	IF EXISTS (
			SELECT 1
			FROM dbo.[tmpKIVLeaveBalance] WITH (NOLOCK)
			WHERE Requestid = @Requestid
			)
	BEGIN
		SELECT @KIVLeaveDay = SUM(TotalDays)
		FROM dbo.tmpKIVLeaveBalance WITH (NOLOCK)
		WHERE Requestid = @Requestid
			AND [Status] = 1

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
		SELECT @KIVLeaveDay = SUM(CASE 
					WHEN IsHalfDay = 1 THEN 0.5
					ELSE 1
					END)
		FROM dbo.LeaveRequestDetail WITH (NOLOCK)
		WHERE RequestId = @RequestId
			AND LeaveType = @KeepInViewLeaveType
	END
	ELSE IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveRequest WITH (NOLOCK)
				WHERE RequestId = @Requestid
					AND LeaveType = @KeepInViewLeaveType
				)
			)
	BEGIN
		SELECT @KIVLeaveDay = TotalDays
		FROM dbo.LeaveRequest WITH (NOLOCK)
		WHERE RequestId = @Requestid
			AND LeaveType = @KeepInViewLeaveType
	END

	IF (ISNULL(@KIVLeaveDay, 0) > 0)
	BEGIN
		--INSERT INTO dbo.[tmpKIVLeaveBalance] (
		--	EmpID
		--	,Requestid
		--	,TotalDays
		--	,InYear
		--	,[Status]
		--	)
		--SELECT TOP 1 @EmpID
		--	,@Requestid
		--	,ISNULL(@KIVLeaveDay, 0)
		--	,YEAR(TODATE)
		--	,0
		--FROM dbo.LeaveRequest lr WITH (NOLOCK)
		--WHERE Requestid = @Requestid

		INSERT INTO dbo.KeepInViewByDayTrans (
			[EmpId]
			,[RequestId]
			,[CreditDays]
			,[DeductDays]
			,[InYear]
			,[InsertedDate]
			)
		SELECT TOP 1 @EmpID
			,@RequestId
			,0
			,ISNULL(@KIVLeaveDay, 0)
			,YEAR(ToDate)
			,GETDATE()
		FROM dbo.LeaveRequest lr WITH (NOLOCK)
		WHERE Requestid = @Requestid
	END
END