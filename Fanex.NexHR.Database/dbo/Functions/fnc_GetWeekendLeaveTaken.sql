
CREATE FUNCTION [dbo].[fnc_GetWeekendLeaveTaken] (
	@EmpId INT
	,@Date DATETIME
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 2)
AS
BEGIN
	DECLARE @WeekedLeaveDayTaken NUMERIC(18, 2)
		,@FromDate DATE = DATEADD(yy, DATEDIFF(yy, 0, @Date), 0)
		,@ToDate DATE = DATEADD (dd, -1, DATEADD(yy, DATEDIFF(yy, 0, @Date) +1, 0))


	SELECT @WeekedLeaveDayTaken = SUM(LeaveDay)
	FROM dbo.WeekendLeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND WorkDate BETWEEN @FromDate
		AND @ToDate
		AND (@RequestId IS NULL OR RequestId <> @RequestId)

	 RETURN ISNULL(@WeekedLeaveDayTaken, 0)
END