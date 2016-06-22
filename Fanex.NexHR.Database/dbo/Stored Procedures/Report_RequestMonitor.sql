
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Report_RequestMonitor] -- '7/1/2014', '8/31/2014', '2,3', 300
	@FromDate DATETIME
	,@ToDate DATETIME
	,@Role NVARCHAR(100)
	,@LoginUserId INT
	,@EmployeeId INT = NULL
	,@GroupIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@MaxRequest INT = 2
AS
/*
Created: Tom.Vo@2013-11-06
Task: show dashboard info for employee
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON

DECLARE @role_lc INT
DECLARE @Groups TABLE (GroupId INT)

IF(@GroupIds <> '')
BEGIN
	INSERT INTO @Groups (GroupId)
	SELECT val FROM fnc_strSplit (@GroupIds, ',')
END

SET @role_lc = CASE 
		WHEN @role LIKE '%6%' THEN 6
		WHEN @role LIKE '%3%' THEN 3
		WHEN @role LIKE '%2%' THEN 2
		END

SELECT @fromDate = CONVERT(VARCHAR(10), @fromDate, 111) + ' 00:00:00'
	,@toDate = CONVERT(VARCHAR(10), @toDate, 111) + ' 23:59:59'

SELECT 
	r.KeepInViewRequest,
	r.TotalChangeShift,
	r.TotalChangeShift,
	r.TotalCheckInOut,
	r.TotalEarlyOut,
	r.TotalLateIn,
	r.TotalLeave,
	r.TotalOvertime,
	r.TotalUnpaidLeave
	,e.EmpId AS Id
	,e.FullName
	,e.NickName
	,g.GroupId
	,g.NAME
	
	
FROM (
	SELECT r.EmpID
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (1, 2) THEN 1
					ELSE 0
					END), 0) AS TotalCheckInOut
		,ISNULL(SUM(CASE 
					WHEN requesttype = 3 THEN 1
					ELSE 0
					END), 0) AS TotalLateIn
		,ISNULL(SUM(CASE 
					WHEN requesttype = 4 THEN 1
					ELSE 0
					END), 0) AS TotalEarlyOut
		,ISNULL(SUM(CASE 
					WHEN requesttype = 5 THEN 1
					ELSE 0
					END), 0) AS TotalChangeShift
		,
		--ISNULL(SUM(case when requesttype = 6  then 1 else 0 end),0) as TotalLeaveRequest2,
		ISNULL(SUM(CASE 
					WHEN requesttype = 6 AND lr.LeaveType NOT IN (21,22,23,25,12,19,20,39)
						AND r.CreatedDate > lr.FromDate THEN 1
					ELSE 0
					END), 0) AS TotalLeave
		,ISNULL(SUM(CASE 
					WHEN requesttype = 6
						AND lr.LeaveType = 25 THEN 1
					ELSE 0
					END), 0) AS TotalUnpaidLeave
		,ISNULL(SUM(CASE 
					WHEN requesttype = 7 THEN 1
					ELSE 0
					END), 0) AS KeepInViewRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 8 THEN 1
					ELSE 0
					END), 0) AS TotalOvertime
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.empid = r.empid
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
	WHERE r.CreatedDate >= @fromdate
		AND r.CreatedDate < @toDate
		AND CurrentStatus IN (4, 6)
		AND (
			(@role_lc IN (6))
			OR (
				@role_lc IN (2, 3)
				AND (
					r.VerifierId = @LoginUserId
					OR r.ApproverId = @LoginUserId
					)
				)
			)
		AND (@EmployeeId IS NULL OR e.EmpId = @EmployeeId)
		AND (@IsLocal IS NULL OR es.IsLocal = @IsLocal)
		AND (@GroupIds = '' OR e.GroupId IN (SELECT GroupId FROM @Groups))
	GROUP BY r.EMPID
	) r
	JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpID = r.EmpID
	LEFT JOIN dbo.[Group] AS G WITH (NOLOCK) ON g.GroupID = e.GroupID
WHERE (
		TotalCheckInOut >= @maxRequest
		OR TotalLateIn >= @maxRequest
		OR TotalEarlyOut >= @maxRequest
		OR TotalChangeShift >= @maxRequest
		OR TotalLeave >= @maxRequest
		OR TotalOvertime >= @maxRequest
		)
	AND (
		e.isDeleted IS NULL
		OR e.isDeleted = 0
		)
	/*
--Report_RequestMonitor '2012/5/1', '2012/5/25',2
*/
