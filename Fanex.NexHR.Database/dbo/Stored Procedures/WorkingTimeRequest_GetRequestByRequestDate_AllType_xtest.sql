

CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByRequestDate_AllType_xtest] 
	@SupervisorID INT=NULL,
	@empid int=null,
	@groupids varchar(max) = '',
	@fromdate datetime,
	@todate datetime,
	@status  varchar(max) = null,
	@excludedEmpID int= null,
	@role varchar(max) = '',
	@isLocal bit = NULL,
	@requestType INT = NULL,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT,
	@isDebug BIT=0
AS
/*
Created: Tom.Vo@2013-11-04
Task: get all emp request list
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON;

SELECT @excludedEmpID = null

SELECT @fromdate = CONVERT(VARCHAR(10),@fromDate,111) + ' 00:00:00',
		@todate = CONVERT(VARCHAR(10),@toDate,111) + ' 23:59:59'

CREATE TABLE #status (Currentstatus TINYINT)
IF ISNULL(@status,'') <>''
	INSERT INTO #status  (Currentstatus)
	SELECT val FROM fnc_strSplit (@status, ',')

CREATE TABLE #RolesList (roleid TINYINT)
DECLARE @RoleType INT --1:Member | 2-Verifier 3-Approver 23 (Verifier+Approver) | 6-HR

IF @SupervisorID IS NOT NULL
BEGIN 
	SET @role = ISNULL(@role,'')
	IF @role <>''
		INSERT INTO #RolesList  (roleid)
		SELECT val FROM fnc_strSplit (@role, ',')
	
	SELECT TOP 1 @role=Roles FROM dbo.Employee ee (NOLOCK) WHERE EmpId = @SupervisorID

	IF EXISTS(SELECT TOP 1 1 FROM #RolesList WHERE roleid = 6)
		BEGIN 
			IF @role LIKE '%6%'
				SET @RoleType=6
			ELSE
				BEGIN
					SELECT @ErrCode=100,@ErrMsg='You don''t have permission on this funcion.'
					RETURN
				END
				--SET @RoleType=1 
		END
	ELSE IF 2=(SELECT COUNT(1) FROM #RolesList WHERE roleid in (2,3) )
		BEGIN 
			IF @role LIKE '%2%' AND @role LIKE '%3%' 
				SET @RoleType=23
			ELSE IF @role LIKE '%2%'
				SET @RoleType=2
			ELSE IF @role LIKE '%3%'
				SET @RoleType=3
			ELSE
				BEGIN
					SELECT @ErrCode=101,@ErrMsg='You don''t have permission on this funcion.'
					RETURN
				END
				--SET @RoleType=1
		END
	ELSE IF EXISTS(SELECT TOP 1 1 FROM #RolesList WHERE roleid in (2) )
		BEGIN
			 IF @role LIKE '%2%'
				SET @RoleType=2
			ELSE
				BEGIN
					SELECT @ErrCode=102,@ErrMsg='You don''t have permission on this funcion.'
					RETURN
				END
				--SET @RoleType=1 
		END
	ELSE IF EXISTS(SELECT TOP 1 1 FROM #RolesList WHERE roleid in (3) )
		BEGIN 
			IF @role LIKE '%3%'
				SET @RoleType=3
			ELSE
				BEGIN
					SELECT @ErrCode=103,@ErrMsg='You don''t have permission on this funcion.'
					RETURN
				END
				--SET @RoleType=1 
		END
	ELSE
		SET @RoleType=1
END
ELSE 
	BEGIN
		SET @RoleType=1
	END

IF (EXISTS(SELECT TOP 1 1 FROM #status WHERE Currentstatus = 1)
	AND EXISTS(SELECT TOP 1 1 FROM #RolesList WHERE roleid = 3))
	BEGIN
		DELETE #status WHERE Currentstatus IN (2)
		INSERT #status (Currentstatus)
			VALUES (2)
	END

CREATE TABLE #GroupsList (GroupId TINYINT)

SET @groupids = ISNULL(@groupids,'')
IF @groupids <>''
	INSERT INTO #GroupsList (GroupId)
	SELECT val FROM fnc_strSplit (@groupids, ',')
ELSE
	BEGIN
		IF(@RoleType = 1 AND NOT EXISTS (SELECT 1 FROM dbo.Employee WHERE EmpId = @SupervisorID AND (Roles LIKE '%6%' OR Roles LIKE '%7%')))
		BEGIN
			INSERT INTO #GroupsList (GroupId)
			SELECT DISTINCT GroupID FROM (
			SELECT GroupID FROM dbo.GroupSupervisor WITH (NOLOCK) WHERE OwnerId = @SupervisorID
			UNION
			SELECT GroupID FROM dbo.Employee WITH (NOLOCK) WHERE EmpId = @SupervisorID) AS a
		END
		ELSE
		BEGIN
			INSERT INTO #GroupsList (GroupId)
			SELECT GRoupID FROM [Group] g WITH (NOLOCK)
		END
	END

SET @fromdate = CONVERT(varchar(10),@fromdate,111)
SET @todate = DATEADD(d, 1, CONVERT(varchar(10),@todate,111))
SET @todate = DATEADD(ms,-2,@todate) -- end of date (23:59:59.999)

CREATE TABLE #TotalDaysRequestInMonth(empid INT,RequestType INT,TotalDaysRequest INT)
CREATE TABLE #RequestInDaily(
			RequestID BIGINT,
			empid INT,
			RequestType INT,
			InOutMins INT,
			RequestedTime DateTime,
			FromDate DateTime,
			ToDate DateTime,
			FromTime DATETIME,
			ToTime DATETIME,
			FromShift INT,
			ToShift INT,
			TotalDays Numeric(18,1),
			IsHalfDay BIT,
			IsWarning BIT)

IF @empid IS NOT NULL
	BEGIN
		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT @empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) 
				ON wt.RequestId = inout.RequestId
			WHERE wt.EmpId = @empid
				and wt.RequestType in (3,4)
				and wt.currentstatus = 6
				-- and (month(inout.FromDate) = month(@fromdate) OR month(inout.ToDate) = month(@todate) )
				AND (DATEDIFF(M, inout.FromDate, @fromdate) = 0 OR DATEDIFF(M, inout.ToDate, @todate) = 0)
		GROUP BY wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT @empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].ChangeShiftRequest cs WITH (NOLOCK) ON wt.RequestId = cs.RequestId
			WHERE wt.EmpId = @empid
				and wt.currentstatus = 6 
				AND (DATEDIFF(M, cs.FromDate, @fromdate) = 0 OR DATEDIFF(M, cs.ToDate, @todate) = 0)
		GROUP BY wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT @empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			WHERE wt.EmpId = @empid
				and wt.currentstatus = 6 
				AND (DATEDIFF(M, lr.FromDate, @fromdate) = 0 OR DATEDIFF(M, lr.ToDate, @todate) = 0)
		GROUP BY wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT @empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].KeepInViewRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			WHERE wt.EmpId = @empid
				and wt.currentstatus = 6 
				AND (DATEDIFF(M, lr.FromDate, @fromdate) = 0 OR DATEDIFF(M, lr.ToDate, @todate) = 0)
		GROUP BY wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT @empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			WHERE wt.EmpId = @empid
				and wt.currentstatus = 6 
				AND (DATEDIFF(M, lr.[OnDate], @fromdate) = 0 OR DATEDIFF(M, lr.[OnDate], @todate) = 0)
		GROUP BY wt.RequestType

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays,InOutMins)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,inout.FromDate,inout.FromDate,inout.ToDate,0,
				case when wt.RequestType = 3 then inout.LateInMins else inout.EarlyOutMins end InOutMins
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) 
				ON wt.RequestId = inout.RequestId
			WHERE wt.EmpId = @empid
				and wt.RequestType in (3,4)
				and ((inout.FromDate >= @fromdate  and inout.FromDate <= @todate )
					OR (inout.ToDate >= @fromdate  and inout.ToDate <= @todate ))
		
		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate, FromTime, ToTime,TotalDays, IsHalfDay)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.FromDate,lr.ToDate,
		CASE WHEN ldt.FromTime IS NULL THEN lr.FromDate ELSE ldt.FromTime END,
		CASE WHEN ldt.ToTime IS NULL THEN lr.ToDate ELSE ldt.ToTime END,
		lr.TotalDays, 1
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
			WHERE wt.EmpId = @empid
				and (( lr.FromDate >= @fromdate and lr.FromDate <= @todate and lr.ToDate >= @todate ) 
				OR ( lr.FromDate <= @fromdate and lr.ToDate >= @todate ) 
				OR ( lr.ToDate >= @fromdate and lr.ToDate <= @todate ) 
				)
				AND lr.TotalDays = 0.5
				AND ldt.IsHalfDay = 1
		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays, IsHalfDay)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.FromDate,lr.ToDate,lr.TotalDays, 0
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			WHERE wt.EmpId = @empid
				and (( lr.FromDate >= @fromdate and lr.FromDate <= @todate and lr.ToDate >= @todate ) 
				OR ( lr.FromDate <= @fromdate and lr.ToDate >= @todate ) 
				OR ( lr.ToDate >= @fromdate and lr.ToDate <= @todate ) 
				)
				AND lr.TotalDays <> 0.5

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate, FromTime, ToTime,TotalDays,FromShift,ToShift, IsHalfDay)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.FromDate,lr.ToDate, lr.FromTime, lr.ToTime,
			CASE WHEN lr.IsHalfDay=0 THEN 1 ELSE 0.5 END TotalDays,lr.ShiftId,lr.ShiftId, lr.IsHalfDay
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].KeepInViewRequest lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			WHERE wt.EmpId = @empid
				and (
					lr.FromDate BETWEEN @fromdate AND @todate
					OR lr.ToDate BETWEEN @fromdate AND @todate
					)

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays,FromShift,IsWarning)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.CheckTime,lr.OnDate,lr.OnDate,1 TotalDays,ec.ShiftID, CASE WHEN ISNULL(att.TotalWorkHours, 0) = 0 THEN 1 ELSE 0 END
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) 
				ON ec.WorkDate = lr.OnDate AND ec.EmpId = lr.EmpId 
			LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON lr.OnDate = att.AttendanceDate AND lr.EmpId = att.EmpId
			WHERE wt.EmpId = @empid
				and lr.[OnDate] >= @fromdate 
				and lr.[OnDate] <= @todate
				AND wt.CurrentStatus = 4

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays,FromShift)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.CheckTime,lr.OnDate,lr.OnDate,1 TotalDays,ec.ShiftID
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) 
				ON ec.WorkDate = lr.OnDate AND ec.EmpId = lr.EmpId 
			WHERE wt.EmpId = @empid
				and lr.[OnDate] >= @fromdate 
				and lr.[OnDate] <= @todate
				AND wt.CurrentStatus <> 4

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays,FromShift,ToShift)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,cs.FromDate,cs.FromDate,cs.ToDate,
			count(1) TotalDays,
			MAX(csd.FromShift) FromShift,
			MAX(csd.ToShift) ToShift
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].ChangeShiftRequest cs  WITH (NOLOCK)	
				ON wt.RequestId = cs.RequestId
			INNER JOIN [dbo].ChangeShiftRequestDetail csd  WITH (NOLOCK)
				ON csd.RequestId = cs.RequestId
			WHERE wt.EmpId = @empid
				and (( cs.FromDate >= @fromdate and cs.FromDate <= @todate and cs.ToDate >= @todate ) 
				OR ( cs.FromDate <= @fromdate and cs.FromDate <= @todate and cs.ToDate >= @todate ) 
				OR ( cs.ToDate >= @fromdate and cs.ToDate <= @todate ) 
				)
		GROUP BY wt.RequestID,wt.EmpId,wt.RequestType,cs.FromDate,cs.ToDate
	END
ELSE
	BEGIN
		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT wt.empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) 
				ON wt.RequestId = inout.RequestId
			INNER JOIN Employee ee WITH (NOLOCK)
				ON ee.EmpId = wt.EmpId
			WHERE wt.currentstatus = 6
				and wt.RequestType in (3,4)
				and ee.GroupId IN (SELECT GroupId FROM #GroupsList )
				AND (DATEDIFF(M, inout.FromDate, @fromdate) = 0 OR DATEDIFF(M, inout.ToDate, @todate) = 0)
		GROUP BY wt.empid,wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT wt.empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			INNER JOIN Employee ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
			WHERE wt.currentstatus = 6
				--and wt.RequestType in (1,2)
				and ee.GroupId IN (SELECT GroupId FROM #GroupsList )
				AND (DATEDIFF(M, lr.FromDate, @fromdate) = 0 OR DATEDIFF(M, lr.ToDate, @todate) = 0)
		GROUP BY wt.empid,wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT wt.empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].ChangeShiftRequest cs WITH (NOLOCK) ON wt.RequestId = cs.RequestId
			WHERE  wt.currentstatus = 6 
				AND (DATEDIFF(M, cs.FromDate, @fromdate) = 0 OR DATEDIFF(M, cs.ToDate, @todate) = 0)
		GROUP BY wt.empid,wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT @empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].KeepInViewRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			WHERE wt.EmpId = @empid
				and wt.currentstatus = 6 
				AND (DATEDIFF(M, lr.FromDate, @fromdate) = 0 OR DATEDIFF(M, lr.ToDate, @todate) = 0)
		GROUP BY wt.RequestType

		INSERT INTO #TotalDaysRequestInMonth (empid,RequestType,TotalDaysRequest) 
		SELECT wt.empid ,wt.RequestType, count(1) 
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			INNER JOIN Employee ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
			WHERE wt.currentstatus = 6
				--and wt.RequestType in (1,2)
				and ee.GroupId IN (SELECT GroupId FROM #GroupsList )
				AND (DATEDIFF(M, lr.OnDate, @fromdate) = 0 OR DATEDIFF(M, lr.OnDate, @todate) = 0)
		GROUP BY wt.empid,wt.RequestType

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,FromDate,ToDate,TotalDays,InOutMins)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,inout.FromDate,inout.ToDate,0,
			case when wt.RequestType = 3 then inout.LateInMins else inout.EarlyOutMins end
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) 
				ON wt.RequestId = inout.RequestId
			WHERE  wt.RequestType in (3,4)
				AND ((inout.FromDate >= @fromdate  and inout.FromDate <= @todate )
					OR (inout.ToDate >= @fromdate  and inout.ToDate <= @todate ))

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,FromDate,ToDate,FromTime, ToTime,TotalDays, IsHalfDay)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.ToDate,
		CASE WHEN ldt.FromTime IS NULL THEN lr.FromDate ELSE ldt.FromTime END,
		CASE WHEN ldt.ToTime IS NULL THEN lr.ToDate ELSE ldt.ToTime END,
		lr.TotalDays, 1
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
			LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
		WHERE (( lr.FromDate >= @fromdate and lr.FromDate <= @todate and lr.ToDate >= @todate ) 
				OR ( lr.FromDate <= @fromdate and lr.FromDate <= @todate and lr.ToDate >= @todate ) 
				OR ( lr.ToDate >= @fromdate and lr.ToDate <= @todate ) 
				)
				AND lr.TotalDays = 0.5
				AND ldt.IsHalfDay = 1

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays, IsHalfDay)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.FromDate,lr.ToDate,lr.TotalDays, 0
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			WHERE  (( lr.FromDate >= @fromdate and lr.FromDate <= @todate and lr.ToDate >= @todate ) 
				OR ( lr.FromDate <= @fromdate and lr.ToDate >= @todate ) 
				OR ( lr.ToDate >= @fromdate and lr.ToDate <= @todate ) 
				)
				AND lr.TotalDays <> 0.5

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate, FromTime, ToTime,TotalDays,FromShift,ToShift, IsHalfDay)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.FromDate,lr.FromDate,lr.ToDate, lr.FromTime, lr.ToTime,
			CASE WHEN lr.IsHalfDay=0 THEN 1 ELSE 0.5 END TotalDays,lr.ShiftId,lr.ShiftId, lr.IsHalfDay
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].KeepInViewRequest lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			WHERE (
					lr.FromDate BETWEEN @fromdate AND @todate
					OR lr.ToDate BETWEEN @fromdate AND @todate
					)
		
		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays,FromShift,IsWarning)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.CheckTime,lr.OnDate,lr.OnDate,1 TotalDays,ec.ShiftID, CASE WHEN ISNULL(att.TotalWorkHours, 0) = 0 THEN 1 ELSE 0 END
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) 
				ON ec.WorkDate = lr.OnDate AND ec.EmpId = lr.EmpId 
			LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON lr.OnDate = att.AttendanceDate AND lr.EmpId = att.EmpId
			WHERE lr.[OnDate] >= @fromdate 
				and lr.[OnDate] <= @todate
				AND wt.CurrentStatus = 4

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,RequestedTime,FromDate,ToDate,TotalDays,FromShift)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,lr.CheckTime,lr.OnDate,lr.OnDate,1 TotalDays,ec.ShiftID
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) 
				ON wt.RequestId = lr.RequestId
			LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) 
				ON ec.WorkDate = lr.OnDate AND ec.EmpId = lr.EmpId 
			WHERE lr.[OnDate] >= @fromdate 
				and lr.[OnDate] <= @todate
				AND wt.CurrentStatus <> 4

		INSERT INTO #RequestInDaily(RequestID,empid,RequestType,FromDate,ToDate,TotalDays,FromShift,ToShift)
		SELECT wt.RequestID,wt.EmpId,wt.RequestType,cs.FromDate,cs.ToDate,
			count(1) TotalDays,
			MAX(csd.FromShift) FromShift,
			MAX(csd.ToShift) ToShift
		FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
			INNER JOIN [dbo].ChangeShiftRequest cs  WITH (NOLOCK)	
				ON wt.RequestId = cs.RequestId
			INNER JOIN [dbo].ChangeShiftRequestDetail csd  WITH (NOLOCK)
				ON csd.RequestId = cs.RequestId
			WHERE (( cs.FromDate >= @fromdate and cs.FromDate <= @todate and cs.ToDate >= @todate ) 
				OR ( cs.FromDate <= @fromdate and cs.FromDate <= @todate and cs.ToDate >= @todate ) 
				OR ( cs.ToDate >= @fromdate and cs.ToDate <= @todate ) 
				)
		GROUP BY wt.RequestID,wt.EmpId,wt.RequestType,cs.FromDate,cs.ToDate
	END

DECLARE @SQL VARCHAR(8000)
DECLARE @RequestTypeSQL VARCHAR(500)
SET @RequestTypeSQL = CASE WHEN @requestType = 0 THEN ''
	WHEN @requestType = 1 THEN ' AND r.RequestType IN (1,2)' + CHAR(13)
	WHEN @requestType = 2 THEN ' AND r.RequestType IN (3,4)' + CHAR(13)
	WHEN @requestType = 3 THEN ' AND r.RequestType = 5' + CHAR(13)
	WHEN @requestType = 4 THEN ' AND r.RequestType = 6' + CHAR(13)
	WHEN @requestType = 5 THEN ' AND r.RequestType = 7' + CHAR(13)
END
DECLARE @Off VARCHAR(3)
SET @SQL = 'SELECT 
			r.RequestId,
			r.RequestType,
			r.Currentstatus AS [Status],
			r.empID,
			rd.FromDate,
			rd.ToDate,
			rd.TotalDays,
			rd.IsHalfDay,
			rd.RequestedTime AS RequestedDate,		
			rd.FromTime,
			rd.ToTime,
			lv.LeaveTypeId,
			lv.Name as LeaveTypeName,
			r.Remark,
			ISNULL(rm.TotalDaysRequest,0) TotalDaysRequestInMonth,

			CASE WHEN r.RequestType IN (1,2,6,7) THEN lr.RefNo 
				WHEN r.RequestType IN (3,4) THEN inout.RefNo 
				WHEN r.RequestType IN (5) THEN NULL
				END RefNo,
			(CASE
				WHEN r.RequestType in (1,2,5,7) THEN	
					s1.ShiftID
				ELSE
					s3.ShiftID
				END	
			) as FromShiftId,
			(CASE
				WHEN r.RequestType in (1,2,5, 7) THEN	
					(CASE WHEN s1.ShiftID IS NULL THEN ' + CHAR(39) + 'OFF' + CHAR(39) + ' ELSE s1.Name END)
				ELSE
					s3.Name
				END	
			) as FromShiftName,
			s2.ShiftID as ToShiftId,
			(CASE WHEN s2.ShiftID IS NULL THEN ' + CHAR(39) + 'OFF' + CHAR(39) +  ' ELSE s2.Name END) AS ToShiftName,
			rd.InOutMins as TotalMinutes,
			r.CreatedDate,
			e.EmpId AS RequesterId,
			e.NickName AS RequesterNickName,
			e.FullName AS RequesterFullName,
			g.GroupId,
			g.Code AS GroupCode,
			g.Name AS GroupName,
			ap_V.EmpId AS VerifierId,
			ap_V.NickName as VerifierNickName,
			ap_V.FullName as VerifierFullName,
			r.VerifiedDate,
			r.VerifiedStatus,
			ap_A.EmpId AS ApproverId,
			ap_A.NickName as ApproverNickName,
			ap_A.FullName as ApproverFullName,
			r.ApprovedDate,
			r.ApprovedStatus,
			ap_H.EmpId AS ProcessedId,
			ap_H.NickName as ProcessedNickName,
			ap_H.FullName as ProcessedFullName,
			r.HRDate AS ProcessedDate,
			r.HRStatus AS ProcessedStatus,
			r.ModifiedBy ModifiedById,
			e_m.NickName as ModifiedByNickName,
			e_m.FullName as ModifiedByFullName,
			r.ModifiedDate,
			ISNULL(rd.IsWarning, 0) AS IsWarning,
			r.ReworkCount		
		FROM  dbo.WorkingTimeRequest r WITH (NOLOCK) 
			INNER JOIN  #RequestInDaily rd ON  r.RequestId = rd.RequestID
			LEFT JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
			LEFT JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
			LEFT JOIN [dbo].[InoutMissRequest] inoutMiss WITH (NOLOCK) ON r.RequestId = inoutMiss.RequestId
			LEFT JOIN [dbo].ChangeShiftRequest csr WITH (NOLOCK) ON r.RequestId = csr.RequestId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy	
			LEFT JOIN  #TotalDaysRequestInMonth rm ON r.empID = rm.empid and r.RequestType = rm.RequestType
			LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID	'+ CHAR(13)
			+ CASE WHEN @isLocal IS NOT NULL THEN 'LEFT JOIN dbo.EmployeeSummary e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId  ' ELSE '' END
+'			LEFT JOIN [dbo].EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),rd.FromDate,111)
			LEFT JOIN dbo.[Shift] s1 WITH (NOLOCK) ON s1.ShiftID= rd.FromShift
			LEFT JOIN dbo.[Shift] s2 WITH (NOLOCK) ON s2.ShiftID= rd.ToShift
			LEFT JOIN dbo.[Shift] s3 WITH (NOLOCK) ON s3.ShiftID = c.ShiftID	

			LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID

			LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.LeaveTypeID = lr.LeaveType

		WHERE (e.isDeleted is null or e.isDeleted = 0)' + CHAR(13)
		+ @RequestTypeSQL
	--+'			and dbo.fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange(rd.FromDate,rd.ToDate,''' 
	--			+ CONVERT(NVARCHAR(20),@fromdate )+''',''' + CONVERT(NVARCHAR(20),@todate)+ ''')=0' + CHAR(13)
	+ CASE WHEN @empid is null THEN '' ELSE '   and r.empid = ' + CONVERT(NVARCHAR(13),@empid)+ '  ' +CHAR(13) END
	+ CASE WHEN @excludedEmpID is null THEN '' ELSE '   and r.empid <> ' + CONVERT(NVARCHAR(13),@excludedEmpID)+ '  ' +CHAR(13) END
	+ CASE WHEN @status is null or @status ='' THEN '' ELSE '			and r.currentstatus IN (SELECT Currentstatus FROM #status ) ' +CHAR(13) END
	+  ' and e.GroupId IN (SELECT GroupId FROM #GroupsList ) ' + CHAR(13)
	+ CASE WHEN @RoleType IN (1,6) THEN ''
			WHEN @RoleType = 2 THEN ' AND r.VerifierId =' + CONVERT(NVARCHAR(13),@SupervisorID) + CHAR(13)
			WHEN @RoleType = 3 THEN ' AND r.ApproverId =' + CONVERT(NVARCHAR(13),@SupervisorID) + CHAR(13)
			WHEN @RoleType = 23 THEN ' AND (r.ApproverId =' + CONVERT(NVARCHAR(13),@SupervisorID) 
										+' OR	r.VerifierId =' + CONVERT(NVARCHAR(13),@SupervisorID) + ')'+ CHAR(13)
			END
	+ CASE WHEN @isLocal IS NOT NULL THEN ' AND  e_s.IsLocal = ' + CONVERT(NVARCHAR(10),@isLocal) + CHAR(13)  ELSE '' END
	+'			ORDER BY r.CreatedDate DESC'
	
	IF @isDebug = 1 PRINT @SQL 
	IF @isDebug = 1 PRINT @RoleType
	EXEC (@SQL )

DROP TABLE #GroupsList
DROP TABLE #TotalDaysRequestInMonth
DROP TABLE #RequestInDaily
DROP TABLE #RolesList
DROP TABLE #status