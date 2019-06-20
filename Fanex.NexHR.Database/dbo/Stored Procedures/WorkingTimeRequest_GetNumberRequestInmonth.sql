/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetNumberRequestInmonth    Script Date: 7/20/2016 10:19:22 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetNumberRequestInmonth    Script Date: 7/19/2016 5:22:22 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetNumberRequestInmonth] -- 300, 5, '2014-08-01', '2014-08-31'
	@EmployeeId INT,
	@RequestType INT,
	@FromDate DATETIME,
	@ToDate DATETIME
AS
/*
Created: 20140729@Quinn.Vo
Task: Get number of request in month.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @Numbers DECIMAL(18, 2)
	SET @Numbers = 0

	IF (@RequestType IN (1, 2))
	BEGIN
		SELECT @Numbers = CAST(COUNT(1) AS DECIMAL)
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.InoutMissRequest AS dt WITH (NOLOCK) ON wk.RequestId = dt.RequestId
		WHERE wk.EmpId = @EmployeeId
			AND RequestType IN (1, 2)
			AND CurrentStatus NOT IN (3, 5, 7)
			AND dt.OnDate BETWEEN @FromDate AND @ToDate
	END
	ELSE IF (@RequestType IN (3, 4))
	BEGIN
		SELECT @Numbers = CAST(COUNT(1) AS DECIMAL)
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.InOutRequest AS dt WITH (NOLOCK) ON wk.RequestId = dt.RequestId
		WHERE wk.EmpId = @EmployeeId
			AND RequestType IN (3, 4)
			AND CurrentStatus NOT IN (3, 5, 7)
			AND dt.FromDate BETWEEN @FromDate AND @ToDate
	END
	ELSE IF (@RequestType = 6)
	BEGIN
		SELECT @Numbers = SUM(lv.TotalDays)
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lv WITH (NOLOCK) ON wk.RequestId = lv.RequestId
		WHERE wk.EmpId = @EmployeeId
			AND RequestType = @RequestType
			AND wk.CurrentStatus NOT IN (3, 5, 7)
			AND (lv.FromDate BETWEEN @FromDate AND @ToDate OR lv.ToDate BETWEEN @FromDate AND @ToDate)
	END
	ELSE IF (@RequestType = 5)
	BEGIN
		SELECT @Numbers = CAST(COUNT(1) AS DECIMAL)
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON wk.RequestId = cs.RequestId
		WHERE wk.EmpId = @EmployeeId
		AND RequestType = @RequestType
		AND CurrentStatus NOT IN (3, 5, 7)
		AND (cs.FromDate BETWEEN @FromDate AND @ToDate OR cs.ToDate BETWEEN @FromDate AND @ToDate)
	END
	ELSE
	BEGIN
		SELECT @Numbers = CAST(COUNT(1) AS DECIMAL)
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON wk.RequestId = kiv.RequestId
		WHERE wk.EmpId = @EmployeeId
		AND RequestType = @RequestType
		AND CurrentStatus NOT IN (3, 5, 7)
		AND kiv.FromDate BETWEEN @FromDate AND @ToDate
	END

	SELECT ISNULL(@Numbers, 0) AS Numbers
END


