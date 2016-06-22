
CREATE PROCEDURE [dbo].[Report_EmployeeDashBoard] -- 125, '2014-07-01'
	@empid int,
	@date datetime
AS
/*
Created: Tom.Vo@2013-10-25
Task: show dashboard info for employee
DB: HRMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON

DECLARE  @month int,  @year int, 
		@groupid int, @AnnualLeaveBalance numeric (18,1), @KIVLeaveBalance numeric (18,1),@OutStandingLeaveBalance numeric (18,1),
		@OutStandingKIVLeaveBalance numeric (18,1),
		@TotalWorkHours decimal (18,2),  
		@TotalWorkDays decimal (18,2),  
		@previousMonth datetime, @fromDate datetime, @toDate datetime,
		@LateIn int,@EarlyOut int,
		@MissIn int, @MissOut int,
		@TotalPendingOvertime INT

SET @date = CONVERT(VARCHAR(10),@date,111)
SET @fromDate = cast(YEAR(@date) as varchar(4)) + '/' + cast(MONTH(@date) as varchar(2)) + '/01'
SET @toDate = dateadd(d,-1, dateadd(m,1,@fromDate))
SET @previousMonth = DATEADD(m,-1,@date )


SELECT @OutStandingLeaveBalance = SUM(TotalDays)
FROM dbo.tmpLeaveBalance e WITH (NOLOCK) 
WHERE e.EmpId= @empid AND e.Status = 0 AND YEAR(@fromDate)=InYear

SELECT @OutStandingKIVLeaveBalance  = SUM(TotalDays)
FROM dbo.tmpKIVLeaveBalance e WITH (NOLOCK) 
WHERE e.EmpId= @empid AND e.Status = 0 AND YEAR(@fromDate)=InYear

SET @AnnualLeaveBalance = dbo.fnc_GetAnnualLeaveBalanceByYear (@empid, YEAR(@fromDate), null)
SELECT --@AnnualLeaveBalance = s.AnnualLeaveBalance, 
		@KIVLeaveBalance = s.KeepInViewBalance, 
		@groupid=e.groupid
FROM dbo.employee e WITH (NOLOCK) 
	inner join dbo.EmployeeSummary s WITH (NOLOCK) 
	on e.EmpId = s.EmpId 
WHERE e.EmpId=@empid

CREATE TABLE #tmpRequest(
	attendancedate DATETIME,
	LateIn BIT,EarlyOut BIT,RequestType INT, RequestId_miss INT,  RequestId_io INT ,InFixed DATETIME, OutFixed DATETIME
 ) 

 INSERT INTO #tmpRequest
 SELECT attendancedate,a.LateIn , a.EarlyOut ,r.RequestType , iom.RequestId ,  ior.RequestId ,InFixed , OutFixed
	FROM dbo.EmployeeAttendance a WITH (NOLOCK) 
	Left Join dbo.WorkingTimeRequest r WITH (NOLOCK) 
		ON a.EmpId = r.EmpId AND r.CurrentStatus IN (1,2)
	left join dbo.InoutMissRequest iom WITH (NOLOCK) 
		On iom.RequestId = r.RequestId
			and iom.OnDate =attendancedate
	left join dbo.InoutRequest ior WITH (NOLOCK) 
		On ior.RequestId = r.RequestId
			and ior.FromDate =attendancedate
	WHERE a.attendancedate between @fromDate and @todate
		and a.empid = @empid
		and a.shiftid is not null
		--and  isnull(Absence,0)=0

DELETE    tmp
FROM #tmpRequest tmp
INNER JOIN #tmpRequest A
	ON A.attendancedate = tmp.attendancedate
WHERE (A.LateIn = 1 AND A.RequestType = 3)
	OR (A.EarlyOut = 1 AND A.RequestType = 4)
	OR (A.InFixed is null AND A.RequestType = 1)
	OR (A.OutFixed is null AND A.RequestType = 2)
	OR (A.OutFixed is null AND CONVERT(NVARCHAR(20),tmp.attendancedate,111)= CONVERT(NVARCHAR(20),GETDATE(),111))

SELECT @LateIn=sum(case when isnull(LateIn,0) > 0 then 1 else 0 end) ,-- isLateIn,
	@EarlyOut= sum(case when isnull(EarlyOut,0) > 0 then 1 else 0 end),-- isEarlyOut,
	@MissIn= sum(case when InFixed is null then 1 else 0 end),-- isMissingCheckin,
	@MissOut=sum(case when OutFixed is null and AttendanceDate < @todate then 1 else 0 end)-- isMissingCheckOut
FROM (
	SELECT DISTINCT attendancedate, LateIn ,EarlyOut ,InFixed , OutFixed		
	FROM #tmpRequest tmp
) tblA

SELECT
		@TotalWorkDays = SUM(CASE WHEN ISNULL(LeaveDay,0)=0 THEN 1 ELSE ISNULL(LeaveDay,0) END)
		,@TotalWorkHours = SUM(TotalWorkHours)
	FROM dbo.EmployeeAttendance a WITH (NOLOCK) 
	WHERE a.attendancedate between @fromDate and @todate
		and a.empid = @empid
		and  isnull(Absence,0)=0

SELECT @TotalPendingOvertime = COUNT(1)
FROM dbo.OTMonthlyRequest WITH (NOLOCK)
WHERE EmpId = @empid
	AND CurrentStatus = 1
	AND (ISNULL(NormalHours, 0) <> 0 OR ISNULL(OffHours, 0) <> 0 OR ISNULL(PublicHours, 0) <> 0)


SELECT 
	ISNULL(@AnnualLeaveBalance,0) AnnualLeaveBalance,
	ISNULL(@KIVLeaveBalance,0) KeepInViewBalance,
	ISNULL(@OutStandingLeaveBalance,0) OutStandingLeaveBalance,
	ISNULL(@OutStandingKIVLeaveBalance,0) OutStandingKIVLeaveBalance,	
	ISNULL(@MissIn+@MissOut,0) ForgotInOut,
	ISNULL(@LateIn+@EarlyOut,0) LateInEarlyOut,
	ISNULL(@TotalWorkHours,0) TotalWorkHours,
	ISNULL(@TotalWorkDays,0) TotalWorkDays,
	
	COUNT(1) TotalRequest,

	ISNULL(SUM(case when requesttype in (1,2) and w.CurrentStatus in (1,2,4,6) then 1 else 0 end),0) as TotalCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and w.CurrentStatus in (1,2) then 1 else 0 end),0) as TotalPendingCheckInOutRequest,

	ISNULL(SUM(case when requesttype in (3,4) and w.CurrentStatus in (1,2,4,6)  then 1 else 0 end),0) as TotalLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and w.CurrentStatus in (1,2)  then 1 else 0 end),0) as TotalPendingLateInEarlyOutRequest,

	ISNULL(SUM(case when requesttype = 5 and w.CurrentStatus in (1,2,4,6) then 1 else 0 end),0) as TotalChangedShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and w.CurrentStatus in (1,2)  then 1 else 0 end),0) as TotalPendingChangedShiftRequest,

	ISNULL(SUM(case when requesttype = 6 and w.CurrentStatus in (1,2,4,6) then 1 else 0 end),0) as TotalLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and w.CurrentStatus in (1,2)  then 1 else 0 end),0) as TotalPendingLeaveRequest,

	ISNULL(SUM(case when requesttype = 7 and w.CurrentStatus in (1,2,4,6) then 1 else 0 end),0) as TotalKIVRequest,
	ISNULL(SUM(case when requesttype = 7 and w.CurrentStatus in (1,2)  then 1 else 0 end),0) as TotalPendingKIVRequest,
	@TotalPendingOvertime AS TotalPendingOverTimeRequest

FROM dbo.WorkingTimeRequest w WITH (NOLOCK) 
WHERE CreatedDate between @fromDate and @todate
	AND empid=@empid

DROP TABLE #tmpRequest