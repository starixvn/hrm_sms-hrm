
CREATE FUNCTION [dbo].[fnc_GetKIVBalance] (
	@EmpID INT
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 2)
AS
BEGIN
	DECLARE @KIVBalance NUMERIC(18, 2)
		,@OutstandingKIVDay NUMERIC(18, 2)

	SELECT @KIVBalance = ISNULL(KeepInViewBalance, 0)
	FROM dbo.EmployeeSummary WITH (NOLOCK)
	WHERE EmpId = @EmpId

	IF(@RequestId IS NOT NULL)
	BEGIN
		SELECT @OutstandingKIVDay = SUM(ISNULL(tmp.TOTALDAYS, 0))
		FROM dbo.tmpKIVLeaveBalance tmp WITH (NOLOCK)
		WHERE tmp.EmpId = @EmpId
			AND tmp.RequestID = @RequestId

		SELECT @KIVBalance = @KIVBalance - ISNULL(@OutstandingKIVDay, 0)
	END

	RETURN ISNULL(@KIVBalance, 0)
END
