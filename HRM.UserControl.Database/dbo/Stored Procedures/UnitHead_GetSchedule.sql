
CREATE PROCEDURE [dbo].[UnitHead_GetSchedule] @FromDate DATE
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

	SELECT SiteId
		,EmpId
		,Code
		,Nickname
		,FullName
		,DepartmentCode
		,DepartmentName
	FROM dbo.UnitHeadInfo WITH (NOLOCK)
	WHERE IsActive = 1
		AND IsDeleted = 0

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
		CROSS APPLY #tempDate AS tmp WITH (NOLOCK)
		LEFT JOIN dbo.UnitHeadSchedule AS uhs WITH (NOLOCK) ON uhs.SiteId = uhinfo.SiteId
		AND uhs.EmpId = uhinfo.EmpId
		AND tmp.WorkDate = uhs.WorkDate
	WHERE uhinfo.IsActive = 1
		AND uhinfo.IsDeleted = 0

	DROP TABLE #tempDate
END