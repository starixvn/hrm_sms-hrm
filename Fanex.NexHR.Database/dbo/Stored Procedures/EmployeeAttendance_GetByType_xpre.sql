
-- [EmployeeAttendance_GetByType] '2015-02-10', '2015-02-22',337, @type = 0
CREATE PROCEDURE [dbo].[EmployeeAttendance_GetByType_xpre] -- '2014-06-01', '2014-06-30', @type=1
	@fromtime datetime, 
	@totime datetime, 
	@empid int = null, 	
	@groupids varchar(MAX) = '',
	@excludeUserDelete int = 1,
	@isLocal bit = NULL,

	@type smallint 
AS

/*
Created: Tom.Vo@2013-10-31
Task: get attentance list of employees by type
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	@type: 0-all, 1-missing checkin; 2-missing check out, 3: latein; 4-earlyout, 5- not work
*/

SET NOCOUNT ON;


SET NOCOUNT ON;

DECLARE @LeaveTypeDefaultId as int , @note varchar(200), @today datetime, @now datetime, @StartDate DATETIME, @EndDate DATETIME
SELECT @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

CREATE TABLE #GroupsList (GroupId TINYINT)

IF ISNULL(@groupids,'') <>''
	INSERT INTO #GroupsList (GroupId)
	SELECT val FROM fnc_strSplit (@groupids, ',')
ELSE
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT GRoupID FROM [Group] g WITH (NOLOCK)
	END

set @now = dateadd(hh,7, GETUTCDATE())
set @today = convert(varchar(10),@now,111) -- use UTC because the server use GMT-4

CREATE TABLE #RequestInDaily(
			RequestID BIGINT,
			empid INT,
			RequestType INT,
			InOutMins INT,
			RequestDate DateTime,
			FromDate DateTime,
			ToDate DateTime,
			FromTime DateTime,
			ToTime DateTime,
			FromShift INT,
			ToShift INT,
			TotalDays NUMERIC(18,2))

--[MissInOutRequest]
INSERT INTO #RequestInDaily(RequestDate,RequestID,empid,RequestType,FromDate,ToDate,TotalDays)
		SELECT wt.CreatedDate RequestDate, wt.RequestID,wt.EmpId,wt.RequestType,
		CASE WHEN DATEDIFF(d, inout.OnDate, inout.CheckTime) = 0 THEN inout.CheckTime ELSE DATEADD(d, -1, inout.CheckTime) END,
		CASE WHEN DATEDIFF(d, inout.OnDate, inout.CheckTime) = 0 THEN inout.CheckTime ELSE DATEADD(d, -1, inout.CheckTime) END,
		0
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].InoutMissRequest inout WITH (NOLOCK) 
				ON wt.RequestId = inout.RequestId
			WHERE  wt.RequestType in (1,2)
				and inout.OnDate >= @fromtime 
				and inout.OnDate <= @totime
				and (@empid is null or wt.empid=@empid)

--[InOutRequest]
INSERT INTO #RequestInDaily(RequestDate,RequestID,empid,RequestType,FromDate,ToDate,TotalDays,InOutMins)
		SELECT wt.CreatedDate RequestDate,wt.RequestID,wt.EmpId,wt.RequestType,inout.FromDate,inout.ToDate,0,
			case when wt.RequestType = 3 then inout.LateInMins else inout.EarlyOutMins end
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) 
				ON wt.RequestId = inout.RequestId
			WHERE  wt.RequestType in (3,4)
				AND (( inout.FromDate >= @fromtime and inout.FromDate <= @totime and inout.ToDate >= @totime ) 
					OR ( inout.FromDate <= @fromtime and inout.ToDate >= @totime ) 
					OR ( inout.ToDate >= @fromtime and inout.ToDate <= @totime ) )
				AND (@empid is null or wt.empid=@empid)

--Keep in view Request
INSERT INTO #RequestInDaily(RequestDate,RequestID,empid,RequestType,FromDate,ToDate, FromTime, ToTime,TotalDays)
		SELECT wt.CreatedDate RequestDate,wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.ToDate, lr.FromTime, lr.ToTime,CASE WHEN lr.IsHalfDay = 1 THEN 0.5 ELSE 1 END
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].KeepInViewRequest lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			WHERE 	(( lr.FromDate >= @fromtime and lr.FromDate <= @totime and lr.ToDate >= @totime ) 
					OR ( lr.FromDate <= @fromtime and lr.ToDate >= @totime ) 
					OR ( lr.ToDate >= @fromtime and lr.ToDate <= @totime ) )
				and (@empid is null or wt.empid=@empid)
--Leave Request <> 0.5
INSERT INTO #RequestInDaily(RequestDate,RequestID,empid,RequestType,FromDate,ToDate,TotalDays)
		SELECT wt.CreatedDate RequestDate,wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.ToDate,lr.TotalDays
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			WHERE (( lr.FromDate >= @fromtime and lr.FromDate <= @totime and lr.ToDate >= @totime ) 
					OR ( lr.FromDate <= @fromtime and lr.ToDate >= @totime ) 
					OR ( lr.ToDate >= @fromtime and lr.ToDate <= @totime ) )
				and (@empid is null or wt.empid=@empid)
				AND lr.TotalDays <> 0.5

--Leave Request = 0.5
INSERT INTO #RequestInDaily(RequestDate,RequestID,empid,RequestType,FromDate,ToDate,TotalDays, FromTime, ToTime)
		SELECT wt.CreatedDate RequestDate,wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.ToDate,lr.TotalDays, 
		(CASE WHEN ldt.FromTime IS NULL THEN lr.FromDate ELSE ldt.FromTime END), 
		(CASE WHEN ldt.ToTime IS NULL THEN lr.ToDate ELSE ldt.ToTime END)
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
			WHERE ((datediff(d, @fromtime, lr.FromDate) <=0 and datediff(d, @totime, lr.ToDate) >=0) 
					OR (datediff(d, @fromtime, lr.FromDate) >=0  and datediff(d, @totime, lr.FromDate) <=0)
					OR (datediff(d, @fromtime, lr.ToDate) >=0  and datediff(d, @totime, lr.ToDate) <=0))
				AND (@empid is null or wt.empid=@empid)
				AND lr.TotalDays = 0.5
--ChangeShiftRequest
INSERT INTO #RequestInDaily(RequestDate,RequestID,empid,RequestType,FromDate,ToDate,TotalDays,FromShift,ToShift)
		SELECT wt.CreatedDate RequestDate,wt.RequestID,wt.EmpId,wt.RequestType,cs.FromDate,cs.ToDate,
			COUNT(1) TotalDays,
			MAX(csd.FromShift) FromShift,
			MAX(csd.ToShift) ToShift
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].ChangeShiftRequest cs  WITH (NOLOCK)	
				ON wt.RequestId = cs.RequestId
			INNER JOIN [dbo].ChangeShiftRequestDetail csd  WITH (NOLOCK)
				ON csd.RequestId = cs.RequestId
			WHERE (( cs.FromDate >= @fromtime and cs.FromDate <= @totime and cs.ToDate >= @totime ) 
					OR ( cs.FromDate <= @fromtime and cs.ToDate >= @totime ) 
					OR ( cs.ToDate >= @fromtime and cs.ToDate <= @totime ) )
				and (@empid is null or wt.empid=@empid)
		GROUP BY wt.CreatedDate,wt.RequestID,wt.EmpId,wt.RequestType,cs.FromDate,cs.ToDate

CREATE TABLE #CalendarDF(AttendanceDate datetime,[Enabled] BIT,IsDeleted BIT)

SET @StartDate = @fromtime
SET @EndDate = @totime

WHILE (DATEPART(dw, @StartDate) <> 2)
BEGIN
	SET @StartDate = DATEADD(d, -1, @StartDate) 
END

WHILE (DATEPART(dw, @EndDate) <> 1)
BEGIN
	SET @EndDate = DATEADD(d, 1, @EndDate) 
END

DECLARE @OutOffDay TABLE (
	WorkDate DATETIME,
	EmpId INT
)
DECLARE @StartDate_tmp DATETIME, @EndDate_tmp DATETIME

WHILE (@StartDate <= @EndDate)
BEGIN
	SET @StartDate_tmp = @StartDate
	SET @EndDate_tmp = DATEADD(d, 6, @StartDate)
	SET @StartDate = DATEADD(d, 7, @StartDate) 

	INSERT INTO @OutOffDay(WorkDate, EmpId)
	SELECT WorkDate, cl.EmpId
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	INNER JOIN 
	(SELECT c.EmpId
		FROM dbo.EmployeeCalendar AS c WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON c.EmpId = es.EmpId
		WHERE WorkDate BETWEEN @StartDate_tmp AND @EndDate_tmp
			AND c.IsOffDay = 1
			AND (@empid IS NULL OR c.EmpId = @empid)
		GROUP BY c.EmpId, es.OffDaysbyWeek
		HAVING COUNT(c.EmpId) > es.OffDaysbyWeek) AS a
	ON cl.EmpId = a.EmpId
	WHERE WorkDate BETWEEN @StartDate_tmp AND @EndDate_tmp
		AND cl.IsOffDay = 1
		AND (@empid IS NULL OR cl.EmpId = @empid)
END

DECLARE @fromtime_tmp datetime=@fromtime
WHILE (@fromtime_tmp<=@totime) 
	BEGIN
		INSERT INTO #CalendarDF(AttendanceDate,[Enabled],IsDeleted) VALUES(@fromtime_tmp,1,0)
		SET @fromtime_tmp=CONVERT(VARCHAR(10),CONVERT(DATETIME,@fromtime_tmp)+1,111)+ ' 00:00:00'
	END

SELECT  T.Code,
		T.EmpId,
		T.FullName,
		T.NickName,
		T.[Team], T.[TeamCode], T.[Rank],
		T.AttendanceDate,
		T.ShiftId,	
		COALESCE(T.ShiftCode,
				--case when ISNULL(T.IsHoliday,0)=0 and T.ShiftId is null and isOffday=1 AND (T.InFixed is null and	T.OutFixed is not null) THEN 'Off day' 
				CASE when ISNULL(T.IsHoliday,0)=0 and T.ShiftId is null and isOffday = 1  THEN 'Off day' 
					WHEN ISNULL(T.IsHoliday,0)=1 AND (T.InFixed is null and	T.OutFixed is null) THEN 'PH' ELSE '' end,'') ShiftCode,
		T.ExpectedInTime,
		T.ExpectedOutTime,
		CASE WHEN T.IsShiftCrossDay=1 AND ISNULL(T.IsHoliday,0)=0 and T.ShiftId is null and isOffday=1 THEN NULL ELSE T.InFixed  END InFixed,
		CASE when T.IsShiftCrossDay=1 AND ISNULL(T.IsHoliday,0)=0 and T.ShiftId is null and isOffday=1 THEN NULL ELSE T.OutFixed END OutFixed,
		T.LateIn,
		T.EarlyOut,
		CASE when T.IsShiftCrossDay=1 AND ISNULL(T.IsHoliday,0)=0 and T.ShiftId is null and isOffday=1 THEN NULL ELSE T.TotalWorkHours END TotalWorkHours,
		T.Absence,
		T.isNotWork,
		T.isOffday,
		(CASE WHEN (
			od.EmpId IS NOT NULL
			OR AttendanceDate1 IS NOT NULL AND isMissingCheckin = 1
			OR AttendanceDate1 IS NOT NULL AND isMissingCheckOut = 1 AND CONVERT(NVARCHAR(20),T.AttendanceDate1,111)< CONVERT(NVARCHAR(20),GETDATE(),111)
		) THEN 1 ELSE 0 END) AS IsWarning,
		T.IsOvertime,
		T.OvertimeHours,
		T.OvertimeStatus,
		ISNULL(stuff(
			case when AttendanceDate1 IS NOT NULL AND (isLateIn=1 AND LateIn > 0) then (', Late In') else '' end +  
			case when AttendanceDate1 IS NOT NULL AND (isEarlyOut=1 AND EarlyOut > 0) then ', Early Out' else '' end   +
			case when AttendanceDate1 IS NOT NULL AND isMissingCheckin=1 then ', Missing Check-in' else '' end   +
			case when AttendanceDate1 IS NOT NULL AND isMissingCheckOut=1 AND CONVERT(NVARCHAR(20),T.AttendanceDate1,111)< CONVERT(NVARCHAR(20),GETDATE(),111) then ', Missing Check-out' else '' end  + 		
			case when AttendanceDate1 IS NOT NULL AND IsKeepInView=1 then ', KIV day' + case when LeaveDay=0.5 then ' (Half day)' else '' end else '' end  + 
			case when ISNULL(T.IsHoliday,0)=0 and isOffday=1 AND ShiftId IS NULL then (CASE WHEN od.EmpId IS NULL THEN  ', Off day' ELSE  ', Off day no match' END) else '' end  + 
			case when ISNULL(T.IsHoliday,0)=0 and isOffday=1 AND ShiftId IS NOT NULL AND od.EmpId IS NOT NULL THEN ', Off day no match' ELSE '' END  + 
			case when ISNULL(T.IsHoliday,0)=1 and ShiftId is null then ', PH' else '' end  + 
			case when isNotWork =1  AND r.LeaveType IS NULL then ', Absent' 
				else case when lv.Name is null or r.LeaveTypeId = @LeaveTypeDefaultId then ''
						else ', ' + lv.Name + CASE WHEN lv2.Name IS NULL THEN '' ELSE ' && ' + lv2.Name END 
									+ case when LeaveDay=0.5 then ' (Half day)' else '' end
					 end
			
				end, 1,2,'') ,CASE WHEN AttendanceDate1 IS NULL THEN 'No Data' ELSE NULL END)
		
		as  Notes,
		r.RequestType,
		r.RequestDate CreatedDate,
		r.FromDate,
		r.ToDate,
		r.TotalDays,
		r.Remark,
		r.LeaveTypeId,
		r.LeaveType,
		r.FromShift,
		r.ToShift,
		r.[Time],
		r.FromTime,
		r.ToTime
from 
	(
	select 
		--a.EmpId,	
		e.Code,
		e.EmpId,
		FullName,
		NickName,
		g.Name [Team], g.Code [TeamCode], g.[Rank],
		CalendarDF.AttendanceDate AttendanceDate,
		a.AttendanceDate AttendanceDate1,
		c.ShiftId,	
		s.code as ShiftCode,
		s.BeginCheckInTime as ExpectedInTime,
		s.BeginCheckOutTime as ExpectedOutTime,
		a.InFixed,
		a.OutFixed,
		a.LateIn,
		a.EarlyOut,
		TotalWorkHours,
		Absence,
		--a.LeaveTypeId,
		--a.LeaveTypeId2,
		--LeaveDay,
		--Notes,
		case when isnull(Absence,0)=0 and isnull(a.LateIn,0) > 0 then 1 else 0 end isLateIn,
		case when isnull(Absence,0)=0 and isnull(a.EarlyOut,0) > 0 then 1 else 0 end isEarlyOut,
		case when isnull(Absence,0)=0 and InFixed is null then 1 else 0 end isMissingCheckin,
		case when isnull(Absence,0)=0 and OutFixed is null /*and AttendanceDate < @today*/ then 1 else 0 end isMissingCheckOut,
		--case when isnull(Absence,0)=1 and a.LeaveTypeId = @LeaveTypeDefaultId --and @now> (AttendanceDate + ' ' + s.EndCheckInTime) -- over the check in time
		--case when isnull(Absence,0)=1 and a.ShiftId	is not null and a.InFixed is null and a.OutFixed is null
		--		then 1 else 0 end isNotWork,
		case when (a.ShiftId IS NOT NULL AND a.InFixed is null AND a.OutFixed is null AND a.TotalWorkHours is null) then 1 else 0 end isNotWork,
		c.isOffday,
		CONVERT(BIT,CASE WHEN ot.EmpId IS NULL THEN 0 ELSE 1 END) IsOvertime,
		(OT.NormalHours	+ OT.OffHours	+ OT.PublicHours) OvertimeHours,
		ot.Status OvertimeStatus,
		c.IsHoliday,
		CASE WHEN --ISNULL(s_CrossDay.IsInAfterDay,0)=1 AND 
			ISNULL(s_CrossDay.IsOutAfterDay,0)=1 THEN 1 ELSE 0 END IsShiftCrossDay
	FROM  #CalendarDF CalendarDF WITH (NOLOCK) 
		LEFT JOIN dbo.Employee e WITH (NOLOCK) ON ((e.isDeleted is null or e.isDeleted = 0) or @excludeUserDelete = 0)	
				AND (
					(@empid is null AND e.[Enabled] = CalendarDF.[Enabled] AND e.[IsDeleted] = CalendarDF.[IsDeleted])
						or e.empid=@empid
					)
		LEFT JOIN dbo.EmployeeSummary e_s WITH (NOLOCK)  ON e_s.EmpId = e.EmpId 
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON CalendarDF.AttendanceDate = a.AttendanceDate 
				AND a.empid = e.empid
				AND (@empid is null or a.empid=@empid)	
		LEFT JOIN dbo.EmployeeCalendar c WITH(NOLOCK) ON (c.WorkDate = CalendarDF.AttendanceDate) 
				AND c.EmpId = e.EmpId
				AND (@empid is null or c.empid=@empid)	
		LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ShiftId=c.ShiftId
		LEFT JOIN dbo.EmployeeCalendar c_CrossDay WITH(NOLOCK) ON (c_CrossDay.WorkDate = (CalendarDF.AttendanceDate-1))
				AND c_CrossDay.EmpId = e.EmpId
				AND (@empid is null or c_CrossDay.empid=@empid)			
		LEFT JOIN dbo.[Shift] s_CrossDay WITH (NOLOCK) ON s_CrossDay.ShiftId=c_CrossDay.ShiftId
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.groupid=g.groupid
		LEFT JOIN dbo.OTDailyRequest ot WITH(NOLOCK) ON a.attendancedate = ot.OvertimeDate and a.EmpId = ot.EmpId
	WHERE  	(@groupids IS NULL OR @groupids = '' OR g.groupid IN (SELECT GroupId FROM #GroupsList))
			AND (@isLocal IS NULL OR e_s.IsLocal = @isLocal )
) T 
LEFT JOIN 
( 
	SELECT
		c.WorkDate,
		a.EmpId, 
		a.RequestType,
		rd.RequestDate,
		rd.FromDate,
		rd.ToDate,
		rd.FromTime,
		rd.ToTime,
		rd.TotalDays,
		a.Remark,
		l.Name as LeaveType,
		(CASE
			WHEN a.RequestType = 5 OR a.RequestType = 7 THEN
				(CASE WHEN s1.ShiftID IS NULL THEN 'OFF' ELSE s1.Name END)
			ELSE
				s3.Name
			END
			) as FromShift,
		(CASE WHEN s2.ShiftID IS NULL THEN 'OFF' ELSE s2.Name END) AS ToShift,
		c.IsKeepInView,
		c.LeaveTypeId,
		c.LeaveTypeId2,
		c.LeaveDay,
		case when a.RequestType = 3 then inout.LateInMins else inout.EarlyOutMins end as [Time]

		FROM WorkingTimeRequest a WITH(NOLOCK)
		INNER JOIN  #RequestInDaily rd ON  a.RequestId = rd.RequestID
		LEFT JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON a.RequestId = lr.RequestId
		LEFT JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) ON a.RequestId = inout.RequestId
		LEFT JOIN dbo.EmployeeCalendar c WITH(NOLOCK) ON (c.WorkDate BETWEEN CONVERT(VARCHAR(10),rd.FromDate, 101)  AND  CONVERT(VARCHAR(10),rd.ToDate, 101)) AND c.EmpId = rd.EmpId
		
		LEFT JOIN dbo.LeaveType l WITH (NOLOCK) ON l.LeaveTypeID = lr.LeaveType
		LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.ShiftId= rd.FromShift
		LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.ShiftId= rd.ToShift
		LEFT JOIN dbo.Shift s3 WITH (NOLOCK) ON s3.ShiftId = c.ShiftID
		
		WHERE a.CurrentStatus IN (4,6)
		AND ((c.LeaveDay <> 0 AND rd.RequestType = 6) OR rd.RequestType <> 6)
) r 
	ON (T.EmpId = r.EmpId AND T.AttendanceDate = r.WorkDate)
LEFT JOIN LeaveType lv WITH (NOLOCK)  ON lv.LeaveTypeId = r.LeaveTypeId
LEFT JOIN LeaveType lv2 WITH (NOLOCK)  ON lv2.LeaveTypeId = r.LeaveTypeId2
LEFT JOIN @OutOffDay AS od ON t.AttendanceDate = od.WorkDate AND t.EmpId = od.EmpId
WHERE @type=0 or 
		(@type =1 and isMissingCheckin =1) or
		(@type =2 and isMissingCheckout=1) or
		(@type =3 and isLateIn =1) or
		(@type =4 and isEarlyOut =1) or
		(@type =5 and isNotWork =1)
		
ORDER BY attendancedate DESC, [Rank] , TeamCode, T.code	

DROP TABLE #GroupsList
DROP TABLE #RequestInDaily
DROP TABLE #CalendarDF