
/*
declare @empid int, @groupid int,
		@todate datetime, @year datetime
set @todate = '2012/06/1'
exec [dbo].[Report_LeaveSummary] 319, '', 2015
*/
CREATE PROCEDURE [dbo].[Report_LeaveSummary_xtest] 
	@empid INT = NULL
	,@groupids VARCHAR(max) = ''
	,@year INT
	,@isLocal BIT = NULL
	,@SupervisorID INT = NULL
	,@role VARCHAR(max) = ''
AS
/*
Created: Tom.Vo@2013-11-21
Task: leave summary report
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #DeductKIV (
		EmpId INT
		,WorkDate DATETIME
		,DeductDays DECIMAL(18, 12)
		)

	CREATE TABLE #tmpOwner (OwnerId INT)

	CREATE TABLE #RolesList (roleid TINYINT)

	DECLARE @RoleType INT --1:Member | 2-Verifier 3-Approver 23 (Verifier+Approver) | 6-HR

	IF @SupervisorID IS NOT NULL
	BEGIN
		SET @role = ISNULL(@role, '')

		IF @role <> ''
			INSERT INTO #RolesList (roleid)
			SELECT val
			FROM fnc_strSplit(@role, ',')

		SELECT TOP 1 @role = Roles
		FROM dbo.Employee ee(NOLOCK)
		WHERE EmpId = @SupervisorID

		IF EXISTS (
				SELECT TOP 1 1
				FROM #RolesList
				WHERE roleid = 6
				)
		BEGIN
			IF @role LIKE '%6%'
				SET @RoleType = 6
			ELSE
				SET @RoleType = 1
		END
		ELSE IF 2 = (
				SELECT COUNT(1)
				FROM #RolesList
				WHERE roleid IN (2, 3)
				)
		BEGIN
			IF @role LIKE '%2%'
				AND @role LIKE '%3%'
			BEGIN
				INSERT INTO #tmpOwner (OwnerId)
				SELECT OwnerId
				FROM [EmployeeSupervisor] WITH (NOLOCK)
				WHERE EmpID = @SupervisorID
					AND RequestType = 6
					AND ActionType IN (2, 3)

				SET @RoleType = 23
			END
			ELSE IF @role LIKE '%2%'
			BEGIN
				INSERT INTO #tmpOwner (OwnerId)
				SELECT OwnerId
				FROM [EmployeeSupervisor] WITH (NOLOCK)
				WHERE EmpID = @SupervisorID
					AND RequestType = 6
					AND ActionType = 2

				SET @RoleType = 2
			END
			ELSE IF @role LIKE '%3%'
			BEGIN
				INSERT INTO #tmpOwner (OwnerId)
				SELECT OwnerId
				FROM [EmployeeSupervisor] WITH (NOLOCK)
				WHERE EmpID = @SupervisorID
					AND RequestType = 6
					AND ActionType = 3

				SET @RoleType = 3
			END
			ELSE
				SET @RoleType = 1
		END
		ELSE IF EXISTS (
				SELECT TOP 1 1
				FROM #RolesList
				WHERE roleid IN (2)
				)
		BEGIN
			IF @role LIKE '%2%'
			BEGIN
				INSERT INTO #tmpOwner (OwnerId)
				SELECT OwnerId
				FROM [EmployeeSupervisor] WITH (NOLOCK)
				WHERE EmpID = @SupervisorID
					AND RequestType = 6
					AND ActionType = 2

				SET @RoleType = 2
			END
			ELSE
				SET @RoleType = 1
		END
		ELSE IF EXISTS (
				SELECT TOP 1 1
				FROM #RolesList
				WHERE roleid IN (3)
				)
		BEGIN
			IF @role LIKE '%3%'
			BEGIN
				INSERT INTO #tmpOwner (OwnerId)
				SELECT OwnerId
				FROM [EmployeeSupervisor] WITH (NOLOCK)
				WHERE EmpID = @SupervisorID
					AND RequestType = 6
					AND ActionType = 3

				SET @RoleType = 3
			END
			ELSE
				SET @RoleType = 1
		END
		ELSE
			SET @RoleType = 1
	END
	ELSE
	BEGIN
		SET @RoleType = 6
	END

	CREATE TABLE #GroupsList (GroupId TINYINT)

	INSERT INTO #GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit(@groupids, ',')

	DECLARE @fromdate DATETIME
		,@todate DATETIME

	-- get the first date of year
	SET @fromdate = cast(@year AS VARCHAR(10)) + '/01/01'
	SET @todate = DATEADD(yy, 1, @fromdate)

	-- get the openning balace for annual leave to first date of year
	SELECT e.EmpId AS Empid
		,SUM(ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0)) AS OpenBalance
	INTO #OpenBalance
	FROM Employee e WITH (NOLOCK)
		LEFT JOIN EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN (
		SELECT *
		FROM dbo.LeaveTrans WITH (NOLOCK)
		WHERE ISNULL(Deleted, 0) = 0
			AND FromDate <= @fromdate
		) t ON e.EmpId = t.Empid
	--LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID=e.GroupId
	WHERE -- (FromDate < @fromdate OR FromDate IS NULL) 
		(
			@EmpId IS NULL
			OR e.EmpId = @empid
			)
		-- AND ISNULL(t.Deleted,0)=0
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
		AND (
			ISNULL(e.isDeleted, 0) = 0
			OR YEAR(ResignedDate) >= year(@todate)
			)
		AND 1 = (CASE 
			WHEN @RoleType = 6 THEN 1
			WHEN @RoleType = 1
				AND e.EmpId = @SupervisorID THEN 1
			WHEN @RoleType IN (2, 3, 23)
				AND EXISTS (
					SELECT TOP 1 1
					FROM #tmpOwner OwnerId
					WHERE e.EmpId = OwnerId
					) THEN 1
			ELSE 0 END
			)
	GROUP BY e.EmpId

	SELECT w.EmpId AS EmpId
		,SUM(CASE 
				WHEN k.IsHalfDay = 1 THEN 0.5
				ELSE 1
				END) AS OpenKIV
	INTO #CreditKIV
	FROM WorkingTimeRequest AS w WITH (NOLOCK)
		INNER JOIN [KeepInViewRequest] AS k WITH (NOLOCK) ON w.RequestId = k.RequestId
	WHERE w.CurrentStatus IN (4, 6)
		AND k.FromDate BETWEEN @fromdate
			AND @todate
	GROUP BY w.EmpId

	-- KIV leave
	INSERT #DeductKIV (
		EmpId
		,WorkDate
		,DeductDays
		)
	SELECT cl.EmpId AS EmpId
		,cl.WorkDate AS WorkDate
		,CAST((CASE 
				WHEN rq.TotalDays > 1 THEN 1
				ELSE rq.TotalDays END
				) AS DECIMAL(18, 2)) AS DeductDays
	FROM EmployeeCalendar AS cl WITH (NOLOCK)
		LEFT JOIN (
		SELECT w.RequestId
			,w.EmpId
			,w.CurrentStatus
			,lv.FromDate
			,lv.ToDate
			,lv.TotalDays
			,lv.LeaveType
		FROM LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN WorkingTimeRequest AS w WITH (NOLOCK) ON lv.RequestId = w.RequestId
		WHERE lv.LeaveType = 10
		) AS rq ON cl.EmpId = rq.EmpId
		AND cl.WorkDate BETWEEN rq.FromDate
			AND rq.ToDate
	WHERE 1 = 1
		-- AND cl.ShiftId IS NOT NULL
		AND cl.IsLeaveDay = 1
		AND cl.WorkDate BETWEEN rq.FromDate
			AND rq.ToDate
		AND rq.CurrentStatus IN (4, 6)
		AND cl.WorkDate BETWEEN @fromdate
			AND DATEADD(dd, - 1, @todate)
	
	-- General leave
	INSERT #DeductKIV (
		EmpId
		,WorkDate
		,DeductDays
		)
	SELECT ldt.EmpId
		,ldt.OnDate AS WorkDate
		,CAST((CASE 
				WHEN IsHalfDay = 1 THEN 0.5
				ELSE 1 END
				) AS DECIMAL(18, 2)) AS DeductDays
	FROM LeaveRequestDetail AS ldt WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lv WITH (NOLOCK) ON ldt.RequestId = lv.RequestId
		INNER JOIN dbo.WorkingTimeRequest AS w WITH (NOLOCK) ON ldt.RequestId = w.RequestId
	WHERE ldt.LeaveType = 10
		AND lv.LeaveType = 0
		AND w.CurrentStatus IN (4, 6)
		AND ldt.OnDate BETWEEN @fromdate
			AND DATEADD(dd, - 1, @todate)

	SELECT EmpId AS EmpId
		,SUM(DeductDays) AS DeductKIVDays
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 1 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Jan
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 2 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Feb
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 3 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Mar
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 4 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Apr
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 5 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS May
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 6 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Jun
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 7 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Jul
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 8 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Aug
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 9 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Sep
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 10 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Oct
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 11 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS Nov
		,SUM(CASE 
				WHEN MONTH(WorkDate) = 12 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) AS [Dec]
	INTO #CurrentKIV
	FROM #DeductKIV
	GROUP BY EmpId

	-- calculate the balance form the first date of year to @todate
	SELECT e.EmpId AS EmpId
		,(ISNULL(CreditDays, 0)) CreditDays
		,SUM(ISNULL(DeductDays, 0)) DeductDays
		,SUM(CASE 
				WHEN MONTH(FromDate) = 1 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Jan
		,SUM(CASE 
				WHEN MONTH(FromDate) = 2 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Feb
		,SUM(CASE 
				WHEN MONTH(FromDate) = 3 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Mar
		,SUM(CASE 
				WHEN MONTH(FromDate) = 4 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Apr
		,SUM(CASE 
				WHEN MONTH(FromDate) = 5 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) May
		,SUM(CASE 
				WHEN MONTH(FromDate) = 6 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Jun
		,SUM(CASE 
				WHEN MONTH(FromDate) = 7 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Jul
		,SUM(CASE 
				WHEN MONTH(FromDate) = 8 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Aug
		,SUM(CASE 
				WHEN MONTH(FromDate) = 9 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Sep
		,SUM(CASE 
				WHEN MONTH(FromDate) = 10 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Oct
		,SUM(CASE 
				WHEN MONTH(FromDate) = 11 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) Nov
		,SUM(CASE 
				WHEN MONTH(FromDate) = 12 THEN ISNULL(DeductDays, 0)
				ELSE 0
				END) DEC
	INTO #CurrenBalace
	FROM Employee e WITH (NOLOCK)
		LEFT JOIN (
		SELECT lt.EmpId
			,CASE 
				WHEN WorkDate IS NULL THEN FromDate
				ELSE WorkDate
				END FromDate
			-- CASE WHEN WorkDate IS NULL THEN DeductDays ELSE LeaveDay END  DeductDays
			,SUM(
				CASE WHEN lt.RequestId IS NULL THEN lt.DeductDays ELSE 
				(CASE WHEN EC.LeaveTypeId = 2 OR EC.LeaveTypeId2 = 2 THEN 
				(CASE WHEN EC.LeaveTypeId2 IS NULL THEN EC.LeaveDay 
					ELSE (CASE WHEN (ISNULL(EC.LeaveTypeId, 0) <> ISNULL(EC.LeaveTypeId2, 0)) THEN 0.5 ELSE 1 END) END)
				ELSE 0 END) END) DeductDays
			--,SUM(CASE 
			--		WHEN WorkDate IS NULL THEN DeductDays
			--		ELSE (CASE 
			--				WHEN DeductDays > 1 THEN 1
			--				ELSE DeductDays END
			--				)
			--		END) DeductDays
		FROM dbo.LeaveTrans lt WITH (NOLOCK)
			LEFT JOIN EmployeeCalendar EC WITH (NOLOCK) ON lt.EmpId = EC.EmpId
			AND ISNULL(DeductDays, 0) <> 0
			AND DATEDIFF(D, lt.FromDate, EC.WorkDate) >= 0 
			AND DATEDIFF(D, EC.WorkDate, lt.ToDate) >= 0 
			AND EC.LeaveDay > 0
		WHERE FromDate >= @fromdate
			AND FromDate < @todate
			AND ISNULL(Deleted, 0) <> 1
		GROUP BY lt.EmpId
			,CASE 
				WHEN WorkDate IS NULL THEN FromDate
				ELSE WorkDate
				END
		) t ON e.EmpId = t.Empid
		LEFT JOIN (
		SELECT lt.EmpId
			,SUM(CreditDays) CreditDays
		FROM dbo.LeaveTrans lt WITH (NOLOCK)
		WHERE FromDate >= @fromdate
			AND FromDate < @todate
			AND ISNULL(Deleted, 0) <> 1
			AND CreditDays > 0
			AND ISNULL(Deleted, 0) <> 1
		GROUP BY lt.EmpId
		) Cred ON e.EmpId = Cred.Empid
		LEFT JOIN EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
	WHERE (
			@EmpId IS NULL
			OR e.EmpId = @empid
			)
		-- AND ISNULL(t.Deleted,0)=0
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
		AND (
			ISNULL(e.isDeleted, 0) = 0
			OR YEAR(ResignedDate) >= year(@todate)
			)
		AND 1 = (CASE 
			WHEN @RoleType = 6 THEN 1
			WHEN @RoleType = 1
				AND e.EmpId = @SupervisorID THEN 1
			WHEN @RoleType IN (2, 3, 23)
				AND EXISTS (
					SELECT TOP 1 1
					FROM #tmpOwner OwnerId
					WHERE e.EmpId = OwnerId
					) THEN 1
			ELSE 0 END
			)
	GROUP BY e.EmpId
		,(ISNULL(CreditDays, 0))
	
	select * from #CurrenBalace
	-- show the report
	SELECT ISNULL(OpenKIV, 0) AS CreditKIV
		,ISNULL(OpenBalance, 0) [AnnualLeaveLastYear]
		,ISNULL(CreditDays, 0) [AnnualLeaveCurrentYear]
		,(cb.Jan + ISNULL(dek.Jan, 0)) AS Jan
		,(cb.Feb + ISNULL(dek.Feb, 0)) AS Feb
		,(cb.Mar + ISNULL(dek.Mar, 0)) AS Mar
		,(cb.Apr + ISNULL(dek.Apr, 0)) AS Apr
		,(cb.May + ISNULL(dek.May, 0)) AS May
		,(cb.Jun + ISNULL(dek.Jun, 0)) AS Jun
		,(cb.Jul + ISNULL(dek.Jul, 0)) AS Jul
		,(cb.Aug + ISNULL(dek.Aug, 0)) AS Aug
		,(cb.Sep + ISNULL(dek.Sep, 0)) AS Sep
		,(cb.Oct + ISNULL(dek.Oct, 0)) AS Oct
		,(cb.Nov + ISNULL(dek.Nov, 0)) AS Nov
		,(cb.[Dec] + ISNULL(dek.[Dec], 0)) AS [Dec]
		,
		--		ISNULL(DeductDays,0) LeaveTaken,
		--ISNULL(OpenBalance,0) + 
		ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) AS LeaveBalance
		,ISNULL(OpenKIV, 0) - ISNULL(dek.DeductKIVDays, 0) AS KIVBalance
		,'' AS Remark
		,cb.EmpId
		,e.Code
		,FullName
		,NickName
		,g.[Rank]
		,JoinedDate
		,ConfirmedDate
		,g.GroupId
		,g.NAME [Name]
	FROM #CurrenBalace cb
		LEFT JOIN #OpenBalance op ON OP.empid = cb.empid
		LEFT JOIN #CreditKIV AS crk ON cb.EmpId = crk.EmpId
		LEFT JOIN #CurrentKIV AS dek ON cb.EmpId = dek.EmpId
		JOIN Employee e WITH (NOLOCK) ON e.EmpId = cb.Empid
		LEFT JOIN EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON g.GroupId = e.GroupId
	WHERE ISNULL(e.Enabled, 0) = 1
		AND (
			@isLocal IS NULL
			OR es.IsLocal = @isLocal
			)
	
	UNION ALL
	SELECT ISNULL(OpenKIV, 0) AS CreditKIV
		,ISNULL(OpenBalance, 0) [AnnualLeaveLastYear]
		,0 AS [AnnualLeaveCurrentYear]
		,dek.Jan
		,dek.Feb
		,dek.Mar
		,dek.Apr
		,dek.May
		,dek.Jun
		,dek.Jul
		,dek.Aug
		,dek.Sep
		,dek.Oct
		,dek.Nov
		,dek.[Dec]
		,
		--		ISNULL(DeductDays,0) LeaveTaken,
		ISNULL(OpenBalance, 0) AS LeaveBalance
		,ISNULL(OpenKIV, 0) - ISNULL(dek.DeductKIVDays, 0) AS KIVBalance
		,'' AS Remark
		,op.EmpId
		,e.Code
		,FullName
		,NickName
		,g.[Rank]
		,JoinedDate
		,ConfirmedDate
		,g.GroupId
		,g.NAME [Name]
	FROM #OpenBalance op
		LEFT JOIN #CreditKIV AS kiv ON op.EmpId = kiv.EmpId
		LEFT JOIN #CurrentKIV AS dek ON op.EmpId = dek.EmpId
		JOIN Employee e WITH (NOLOCK) ON e.Empid = op.Empid
		LEFT JOIN EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON g.GroupId = e.GroupId
	WHERE e.Empid NOT IN (
			SELECT EmpID
			FROM #CurrenBalace cb
			)
		AND ISNULL(e.Enabled, 0) = 1
		AND (
			@isLocal IS NULL
			OR es.IsLocal = @isLocal
			)
	ORDER BY [Rank]
		,[Name]
		,code

	DROP TABLE #OpenBalance

	DROP TABLE #CurrenBalace

	DROP TABLE #CreditKIV

	DROP TABLE #DeductKIV

	DROP TABLE #CurrentKIV

	DROP TABLE #GroupsList
END
