
CREATE FUNCTION [dbo].[fnc_GetAnnualLeaveBalanceByYear] (
	@EmpID INT
	,@Year INT
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 2)
AS
BEGIN
	DECLARE @LeaveBalanceDay NUMERIC(18, 2)
		,@OutstandingLeaveDay NUMERIC(18, 2)

	SELECT @OutstandingLeaveDay = SUM(ISNULL(tmp.TOTALDAYS, 0))
	FROM dbo.tmpLeaveBalance tmp WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest lr WITH (NOLOCK) ON tmp.EmpId = lr.EmpId
		AND tmp.RequestID = lr.RequestId
	WHERE tmp.EmpId = @EmpId
		AND tmp.[Status] = 0
		AND tmp.InYear = @Year
		AND (@RequestId IS NULL OR tmp.RequestID <> @RequestId)

	SELECT @LeaveBalanceDay = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpID = @EmpID
		AND YEAR(FromDate) = @Year
		AND ISNULL(Deleted, 0) <> 1

	SELECT @LeaveBalanceDay = ISNULL(@LeaveBalanceDay, 0) - ISNULL(@OutstandingLeaveDay, 0)

	RETURN ISNULL(@LeaveBalanceDay, 0)
END
