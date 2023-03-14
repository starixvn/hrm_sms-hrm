
CREATE PROCEDURE [dbo].[Sync_GO_UnitHead_GetSchedule]
AS
/*
Created: 20230227@Tim.Trinh
Task: get unit head schedule
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATE = GETDATE()
		,@ToDate DATE = DATEADD(MONTH, 1, GETDATE())

	CREATE TABLE #tempUnitHead (
		EmpId INT PRIMARY KEY
		,UserId INT
		)

	INSERT INTO #tempUnitHead (
		EmpId
		,UserId
		)
	SELECT emp.EmpId
		,emp.UserId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.Code IN ('00031', '00036', '00053', '00090')
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0

	SELECT empca.EmpId
		,empca.WorkDate
		,CASE 
			WHEN empca.IsLeaveDay = 0
				AND empca.ShiftId IS NOT NULL THEN 1
			ELSE 0
			END AS IsWorkday
		,CASE 
			WHEN empca.IsLeaveDay = 1
				OR empca.ShiftId IS NOT NULL
				OR empca.IsHoliday = 1 THEN 0
			ELSE empca.IsOffDay
			END AS IsOffDay
		,CASE 
			WHEN empca.IsLeaveDay = 1
				OR empca.ShiftId IS NOT NULL THEN 0
			ELSE empca.IsHoliday
			END AS IsHoliday
		,empca.IsLeaveDay
		,CASE 
			WHEN empca.LeaveTypeId IS NOT NULL THEN ltype.Code
			WHEN empca.LeaveTypeId2 IS NOT NULL THEN ltype2.Code
			END AS LeaveTypeCode
		,CASE 
			WHEN empca.LeaveTypeId IS NOT NULL THEN ltype.[Name]
			WHEN empca.LeaveTypeId2 IS NOT NULL THEN ltype2.[Name]
			END AS LeaveTypeName
	FROM dbo.tmpEmployeeCalendar AS empca WITH (NOLOCK)
		LEFT JOIN dbo.LeaveType AS ltype WITH (NOLOCK) ON empca.LeaveTypeId = ltype.LeaveTypeId
		LEFT JOIN dbo.LeaveType AS ltype2 WITH (NOLOCK) ON empca.LeaveTypeId2 = ltype2.LeaveTypeId
	WHERE empca.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND EXISTS (
			SELECT 1
			FROM #tempUnitHead AS tmp WITH (NOLOCK)
			WHERE empca.EmpId = tmp.EmpId
			)
		AND (
			empca.RequestTypeId IS NULL
			OR empca.RequestTypeId = 6 -- leave
			)

	DROP TABLE #tempUnitHead
END