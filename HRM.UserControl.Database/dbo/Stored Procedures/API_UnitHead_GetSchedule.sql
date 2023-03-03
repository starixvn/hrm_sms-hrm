
CREATE PROCEDURE [dbo].[API_UnitHead_GetSchedule] @FromDate DATE
	,@ToDate DATE
AS
/*  
Created: 20230227@Tim.Trinh
Task: get unit head schedule
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tempDate DATE = @fromDate

	CREATE TABLE #tempDate (WorkDate DATE PRIMARY KEY)

	WHILE @tempDate <= @toDate
	BEGIN
		INSERT INTO #tempDate (WorkDate)
		VALUES (@tempDate)

		SET @tempDate = DATEADD(d, 1, @tempDate)
	END

	SELECT uhinfo.SiteId
		,s.CompanyId
		,uhinfo.EmpId
		,tmp.WorkDate
		,uhs.IsWorkDay
		,uhs.IsOffDay
		,uhs.IsHoliday
		,uhs.IsLeaveDay
		,uhs.LeaveTypeCode
		,uhs.LeaveTypeName
	FROM dbo.UnitHeadInfo AS uhinfo WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON uhinfo.SiteId = s.SiteId
		CROSS APPLY #tempDate AS tmp WITH (NOLOCK)
		LEFT JOIN dbo.UnitHeadSchedule AS uhs WITH (NOLOCK) ON uhs.SiteId = uhinfo.SiteId
		AND uhs.EmpId = uhinfo.EmpId
		AND tmp.WorkDate = uhs.WorkDate
	WHERE uhinfo.IsActive = 1
		AND uhinfo.IsDeleted = 0

	DROP TABLE #tempDate
END