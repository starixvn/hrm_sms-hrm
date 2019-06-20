/****** Object:  User Defined Function dbo.fnc_GetKIVBalance    Script Date: 7/20/2016 10:18:58 AM ******/
/****** Object:  User Defined Function dbo.fnc_GetKIVBalance    Script Date: 7/19/2016 5:22:10 PM ******/

CREATE FUNCTION [dbo].[fnc_GetKIVBalance] (
	@EmpID INT
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 2)
AS
BEGIN
	DECLARE @KIVBalance NUMERIC(18, 2) = 0
		,@OutstandingKIVDay NUMERIC(18, 2)
		,@fromdate DATETIME
		,@todate DATETIME
		,@currentYear INT
		,@creditKIV NUMERIC(18, 2)
		,@deductKIV NUMERIC(18, 2)
	
	SET @currentYear =  DATEPART(YYYY,GETDATE())
	SET @fromdate = cast(@currentYear AS VARCHAR(10)) + '/01/01'
	SET @todate = DATEADD(yy, 1, @fromdate)
	
	SELECT @creditKIV = SUM(CASE 
					WHEN k.IsHalfDay = 1 THEN 0.5
					ELSE 1
					END) 
	FROM dbo.[KeepInViewRequest] AS k WITH (NOLOCK), dbo.WorkingTimeRequest AS w WITH (NOLOCK)
	WHERE w.EmpId = @EmpID
		AND w.RequestId = k.RequestId 
		AND w.CurrentStatus IN (4, 6)
		AND (k.OnDate BETWEEN @fromdate AND @todate)
	
	SELECT @deductKIV = SUM(ISNULL(tmp.TOTALDAYS, 0))
	FROM dbo.tmpKIVLeaveBalance tmp WITH (NOLOCK)
	WHERE tmp.EmpId = @EmpID 
		AND (tmp.Status = 1 OR tmp.Status = 0)
		AND InYear = @currentYear

	SET @KIVBalance = ISNULL(@creditKIV, 0) - ISNULL(@deductKIV , 0)
	
	SET @KIVBalance = CASE WHEN @KIVBalance > 0 THEN @KIVBalance ELSE 0 END
	------------------------------
	IF(@RequestId IS NOT NULL)
	BEGIN
		SELECT @OutstandingKIVDay = SUM(ISNULL(tmp.TOTALDAYS, 0))
		FROM dbo.tmpKIVLeaveBalance tmp WITH (NOLOCK)
		WHERE tmp.EmpId = @EmpId
			AND tmp.RequestID = @RequestId

		SELECT @KIVBalance = @KIVBalance + ISNULL(@OutstandingKIVDay, 0)
	END

	RETURN ISNULL(@KIVBalance, 0)
END
