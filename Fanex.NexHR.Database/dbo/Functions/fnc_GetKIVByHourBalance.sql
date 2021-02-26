
CREATE FUNCTION [dbo].[fnc_GetKIVByHourBalance] (
	@EmpID INT
	,@Date DATETIME
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 2)
AS
BEGIN
	DECLARE @KIVBalance NUMERIC(18, 2) = 0
		,@FromDate DATE

	SELECT @FromDate = DATEADD(DAY, -1, DATEADD(MONTH, -1, @Date))

	--SELECT @KIVBalance = SUM(ISNULL(CreditMinutes, 0)) - SUM(CASE 
	--			WHEN @RequestId IS NULL THEN ISNULL(DeductMinutes, 0)
	--			ELSE CASE 
	--					WHEN RequestId = @RequestId THEN 0
	--					ELSE ISNULL(DeductMinutes, 0)
	--					END
	--			END)

	SELECT @KIVBalance = SUM(ISNULL(MinutesBalance, 0))
	FROM dbo.KeepInViewByHourTrans WITH (NOLOCK)
	WHERE EmpId = @EmpID
		AND WorkDate > @FromDate

	RETURN ISNULL(@KIVBalance, 0)
END