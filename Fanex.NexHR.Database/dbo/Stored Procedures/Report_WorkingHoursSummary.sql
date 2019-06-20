
/****** Object:  Stored Procedure dbo.Report_WorkingHoursSummary    Script Date: 7/20/2016 10:19:18 AM ******/
/****** Object:  Stored Procedure dbo.Report_WorkingHoursSummary    Script Date: 7/19/2016 5:22:20 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/*
declare @fromdate datetime,
		@todate datetime,
		@empid int,
		@groupid int
set @fromdate = '2014/09/01'
set @todate = '2014/09/30'
set @empid=245
EXEC Report_WorkingHoursSummary @fromdate , @todate ,	@empid , @groupid 
*/
CREATE PROCEDURE [dbo].[Report_WorkingHoursSummary] @fromdate DATETIME
	,@todate DATETIME
	,@empid INT = NULL
	,@groupids VARCHAR(max) = ''
	,@isLocal BIT = NULL
AS
/*
Created:  Tom.Vo@2013-11-21
Task: Working Hours Summary REPORT
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @AnnualLeaveType INT
	,@UnpaidLeaveType INT
	,@UnpaidInsuranceType INT
	,@PeternityLeaveType INT
	,@MaternityLeaveType INT
	,@BusinessTripType INT
	,@BereavementLeaveType INT
	,@CompensationleaveType INT
	,@WeddingLeaveType INT

SELECT @AnnualLeaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'AL'

SELECT @UnpaidLeaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'UPL'

SELECT @UnpaidInsuranceType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'UPLI'

SELECT @PeternityLeaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'PL'

SELECT @MaternityLeaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'ML'

SELECT @BusinessTripType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'BTL'

SELECT @BereavementLeaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'BL3'

SELECT @CompensationleaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'Cl'

SELECT @WeddingLeaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'WL'

CREATE TABLE #GroupsList (GroupId INT)

INSERT INTO #GroupsList (GroupId)
SELECT val
FROM fnc_strSplit(@groupids, ',')

SELECT es.WorkingDays AS [WorkingDay]
	,SUM(CASE 
			WHEN (
					ISNULL(a.TotalWorkHours, 0) = 0
					OR (
						c.IsLeaveDay = 1
						AND c.LeaveDay = 1
						)
					)
				THEN 0
			ELSE (
					CASE 
						WHEN c.IsLeaveDay = 0
							THEN 1
						ELSE (
								CASE 
									WHEN c.LeaveDay < 1
										THEN 0.5
									ELSE 0
									END
								)
						END
					)
			END) AS [ActualWorkingDay]
	,SUM(CASE 
			WHEN (
					c.IsOffDay = 1
					OR c.IsHoliday = 1
					OR (
						c.IsLeaveDay = 1
						AND c.LeaveDay = 1
						)
					)
				THEN 0
			ELSE (
					CASE 
						WHEN c.IsLeaveDay = 0
							THEN 1
						ELSE (
								CASE 
									WHEN c.LeaveDay < 1
										THEN 0.5
									ELSE 0
									END
								)
						END
					)
			END) AS PaymentWorkingDay
	,es.WorkingHours
	--sum(case when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=0 then 1 
	--		when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=1 and c.LeaveDay < 1 then 0.5
	--		else 0 end) [ActualWorkingDay],
	,sum(CASE 
			WHEN isOffday = 0
				AND isHoliday = 0
				AND IsLeaveDay = 1
				AND (
					c.LeaveTypeId = @AnnualLeaveType
					OR (
						c.LeaveTypeId2 IS NOT NULL
						AND c.LeaveTypeId2 = @AnnualLeaveType
						)
					)
				THEN CASE 
						WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
							AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
							THEN 0.5
						ELSE c.LeaveDay
						END
			ELSE 0
			END) [AnnualLeave]
	,
	-- sum(case when isOffday=0 and isHoliday=0 and IsLeaveDay=1 and c.LeaveTypeId = @UnpaidLeaveType  then c.LeaveDay else 0 end) [UnpaidLeave],
	SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @UnpaidLeaveType
						OR c.LeaveTypeId2 = @UnpaidLeaveType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [UnpaidLeave]
	,SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @BusinessTripType
						OR c.LeaveTypeId2 = @BusinessTripType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [BusinessTrip]
	,SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @BereavementLeaveType
						OR c.LeaveTypeId2 = @BereavementLeaveType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [BereavementLeave]
	,SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @CompensationleaveType
						OR c.LeaveTypeId2 = @CompensationleaveType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [Compensationleave]
	,SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @WeddingLeaveType
						OR c.LeaveTypeId2 = @WeddingLeaveType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [WeddingLeave]
	,SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @UnpaidLeaveType
						OR c.LeaveTypeId2 = @UnpaidLeaveType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [UnpaidLeaveInsurance]
	,SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @PeternityLeaveType
						OR c.LeaveTypeId2 = @PeternityLeaveType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [PeternityLeave]
	,SUM((
			CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						c.LeaveTypeId = @MaternityLeaveType
						OR c.LeaveTypeId2 = @MaternityLeaveType
						)
					THEN CASE 
							WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
								AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0)
								THEN 0.5
							ELSE c.LeaveDay
							END
				ELSE 0
				END
			)) AS [MaternityLeave]
	,SUM(ISNULL(a.LateIn, 0)) LateInMins
	,SUM(ISNULL(a.EarlyOut, 0)) EarlyOutMins
	,SUM(ISNULL(a.TotalWorkHours, 0)) TotalHours
	,SUM(a.TotalWorkHours) / sum(CASE 
			WHEN Absence IS NOT NULL
				AND Absence = 0
				AND IsLeaveDay = 0
				THEN 1
			WHEN Absence IS NOT NULL
				AND Absence = 0
				AND IsLeaveDay = 1
				AND c.LeaveDay < 1
				THEN 0.5
			ELSE NULL
			END) AS AverageHours
	,SUM(ISNULL(ot.NormalHours, 0)) AS [OTNormalHours]
	,SUM(ISNULL(ot.OffHours, 0)) AS [OTOffHours]
	,SUM(ISNULL(ot.PublicHours, 0)) AS [OTPublicHours]
	,c.EmpId
	,e.Code
	,FullName
	,NickName
	,g.GroupId
	,g.Code [Name]
FROM dbo.EmployeeAttendance a WITH (NOLOCK)
RIGHT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = a.empid
	AND c.workdate = a.AttendanceDate
LEFT JOIN dbo.OTDailyRequest ot WITH (NOLOCK) ON ot.EmpId = a.EmpId
	AND c.WorkDate = ot.OvertimeDate
	AND ot.[Status] = 6 --Approved
JOIN dbo.employee e WITH (NOLOCK) ON e.empid = c.empid
JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.empid = es.empid
LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON g.GroupId = e.GroupId
WHERE (
		@empid IS NULL
		OR c.empid = @empid
		)
	AND (
		e.isDeleted IS NULL
		OR e.isDeleted = 0
		)
	AND WorkDate BETWEEN @fromdate
		AND @todate
	AND (
		@isLocal IS NULL
		OR es.IsLocal = @isLocal
		)
	AND (
		@groupids IS NULL
		OR @groupids = ''
		OR e.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
		)
GROUP BY c.empid
	,e.Code
	,es.WorkingDays
	,es.WorkingHours
	,e.FullName
	,NickName
	,g.GroupId
	,g.Code
	,[Rank]
ORDER BY [Rank]
	,g.Code
	,e.code

DROP TABLE #GroupsList
