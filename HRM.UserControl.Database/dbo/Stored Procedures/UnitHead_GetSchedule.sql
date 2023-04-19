
CREATE PROCEDURE [dbo].[UnitHead_GetSchedule] @CompanyId SMALLINT = NULL
	,@FromDate DATE
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

	DECLARE @tempDate DATE = @FromDate

	CREATE TABLE #tempDate (WorkDate DATE PRIMARY KEY)

	WHILE @tempDate <= @ToDate
	BEGIN
		INSERT INTO #tempDate (WorkDate)
		VALUES (@tempDate)

		SET @tempDate = DATEADD(d, 1, @tempDate)
	END

	SELECT uhi.SiteId
		,c.CompanyId
		,c.[Name] AS CompanyName
		,uhi.EmpId
		,uhi.Code
		,uhi.Nickname
		,uhi.FullName
		,uhi.DepartmentCode
		,uhi.DepartmentName
	FROM dbo.UnitHeadInfo AS uhi WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON uhi.SiteId = s.SiteId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
	WHERE uhi.IsActive = 1
		AND uhi.IsDeleted = 0
		AND (@CompanyId IS NULL OR s.CompanyId = @CompanyId)
	ORDER BY uhi.Nickname

	SELECT uhinfo.SiteId
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
		AND (@CompanyId IS NULL OR s.CompanyId = @CompanyId)

	DROP TABLE #tempDate
END