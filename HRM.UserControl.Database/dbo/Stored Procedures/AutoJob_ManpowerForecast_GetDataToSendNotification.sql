
CREATE PROCEDURE [dbo].[AutoJob_ManpowerForecast_GetDataToSendNotification]
AS
/*
Created: 20230721@Tim.Trinh
Task: get manpower forecast to send noti
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT p.PeriodId
		,p.[Name]
		,p.[Year]
	FROM dbo.Manpower_Period AS p WITH (NOLOCK)
	WHERE p.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM dbo.Manpower_PeriodProcessSetting AS ps WITH (NOLOCK)
			WHERE p.PeriodId = ps.PeriodId
				AND ps.FromDate = @Today
			)

	SELECT DISTINCT emp.EmpId
		,emp.Nickname
		,emp.FullName
		,emp.Email
		,ps.PeriodId
		,ps.ProcessId
		,ps.FromDate
		,ps.ToDate
	FROM dbo.Manpower_Period AS p WITH (NOLOCK)
		INNER JOIN dbo.Manpower_PeriodProcessSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
		INNER JOIN dbo.Manpower_Flow AS f WITH (NOLOCK) ON ps.ProcessId = f.ProcessId
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON f.SiteId = emp.SiteId
		AND f.EmpId = emp.EmpId
	WHERE p.IsActive = 1
		AND ps.FromDate = @Today
		AND emp.IsActive = 1
		AND emp.IsDeleted = 0
END