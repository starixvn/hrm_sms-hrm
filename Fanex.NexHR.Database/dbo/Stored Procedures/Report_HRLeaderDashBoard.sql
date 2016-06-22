/*
--Report_HRLeaderDashBoard 150, '2012/5/15'
*/

CREATE PROCEDURE [dbo].[Report_HRLeaderDashBoard] -- 300, '2014-07-01'
	@Empid int = null,
	@date datetime
AS
/*
Created:  Tom.Vo@2013-10-25
Task: show dashboard info for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON

DECLARE @fromDate datetime, @toDate datetime, @role int, @roles varchar(20), @TotalOvertimePendingByVerifier INT, @TotalOvertimePendingByApprover INT


SET @date = CONVERT(VARCHAR(10),@date,111)
SET @fromDate = dateadd(d,-7,@date)
SET @toDate = dateadd(d,1,@date)

SELECT @roles=roles
FROM dbo.Employee e WITH (NOLOCK)
WHERE e.EmpId = @Empid

DECLARE @tblrole TABLE(RoleId INT,Title NVARCHAR(100))

INSERT INTO @tblrole (RoleId,Title)
SELECT cast(val as int) RoleId,ld.Name AS Title
FROM [dbo].[Lookup] l WITH(NOLOCK) 
	INNER JOIN 	[dbo].[LookupData] ld WITH(NOLOCK) ON l.Id = ld.LookupId
	INNER JOIN dbo.fnc_strSplit(@roles, ',') fn		ON fn.val = ld.Value
WHERE (l.Code = 'ROLE' AND ld.Code ='AP') OR @roles = '7'

if (@@ROWCOUNT<1)
	return;

CREATE TABLE #Report (RoleId INT,RoleTitle Nvarchar(100),
			TotalPendingCheckInOutRequest INT,TotalApprovedCheckInOutRequest INT,TotalRejectCheckInOutRequest INT,
			TotalPendingLateInEarlyOutRequest INT,TotalApprovedLateInEarlyOutRequest INT,TotalRejectLateInEarlyOutRequest INT,
			TotalPendingChangeShiftRequest INT,TotalApprovedChangeShiftRequest INT,TotalRejectChangeShiftRequest INT,
			TotalPendingLeaveRequest INT,TotalApprovedLeaveRequest INT,TotalRejectLeaveRequest INT,
			TotalPendingKIVRequest INT,TotalApprovedKIVRequest INT,TotalRejectKIVRequest INT, TotalPendingOvertime INT
)

CREATE TABLE #Data (Verifier Nvarchar(100), Approver  Nvarchar(100),
			TotalPendingCheckInOutRequest INT,TotalApprovedCheckInOutRequest INT,TotalRejectCheckInOutRequest INT,
			TotalPendingLateInEarlyOutRequest INT,TotalApprovedLateInEarlyOutRequest INT,TotalRejectLateInEarlyOutRequest INT,
			TotalPendingChangeShiftRequest INT,TotalApprovedChangeShiftRequest INT,TotalRejectChangeShiftRequest INT,
			TotalPendingLeaveRequest INT,TotalApprovedLeaveRequest INT,TotalRejectLeaveRequest INT,
			TotalPendingKIVRequest INT,TotalApprovedKIVRequest INT,TotalRejectKIVRequest INT
)

--INSERT LIST REQUEST PENDING
INSERT INTO #data (
			Verifier,Approver,
			TotalPendingCheckInOutRequest,TotalApprovedCheckInOutRequest,TotalRejectCheckInOutRequest,
			TotalPendingLateInEarlyOutRequest,TotalApprovedLateInEarlyOutRequest,TotalRejectLateInEarlyOutRequest,
			TotalPendingChangeShiftRequest,TotalApprovedChangeShiftRequest,TotalRejectChangeShiftRequest,
			TotalPendingLeaveRequest,TotalApprovedLeaveRequest,TotalRejectLeaveRequest,
			TotalPendingKIVRequest,TotalApprovedKIVRequest,TotalRejectKIVRequest
			)
SELECT 
	CASE WHEN w.VerifierId = @Empid THEN 'Verifier' ELSE '' END Verifier ,
	CASE WHEN w.ApproverId = @Empid THEN 'Approver' ELSE '' END Approver ,
	-- check in/out request	
	ISNULL(SUM(case when requesttype in (1,2) and  
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				then 1 else 0 end),0) as TotalPendingCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and  
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				then 1 else 0 end),0) as TotalApprovedCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and  
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				then 1 else 0 end),0) as TotalRejectCheckInOutRequest,

	-- late in/early out request	
	ISNULL(SUM(case when requesttype in (3,4) and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectLateInEarlyOutRequest,

	-- Change Shift Request	
	ISNULL(SUM(case when requesttype = 5 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingChangeShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedChangeShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectChangeShiftRequest,
	
	--Leave Request	
	ISNULL(SUM(case when requesttype = 6 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectLeaveRequest,

	--KIV Request	
	ISNULL(SUM(case when requesttype = 7 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingKIVRequest,
	ISNULL(SUM(case when requesttype = 7 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedKIVRequest,
	ISNULL(SUM(case when requesttype = 7 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectKIVRequest
FROM dbo.WorkingTimeRequest w WITH(NOLOCK)
	INNER JOIN 	[dbo].[LookupData] ld WITH(NOLOCK) ON w.RequestType = ld.Value
	INNER JOIN [dbo].[Lookup] l WITH(NOLOCK) ON l.Id = ld.LookupId
WHERE w.EmpId <> @Empid
	AND l.Code = 'RT' 
	AND (w.VerifierId = @Empid OR w.ApproverId = @Empid )
	AND CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
GROUP BY
	CASE WHEN w.VerifierId  = @Empid THEN 'Verifier' ELSE '' END,
	CASE WHEN w.ApproverId = @Empid THEN 'Approver' ELSE '' END

--INSERT LIST REQUEST APPROVED
INSERT INTO #data (
			Verifier,Approver,
			TotalPendingCheckInOutRequest,TotalApprovedCheckInOutRequest,TotalRejectCheckInOutRequest,
			TotalPendingLateInEarlyOutRequest,TotalApprovedLateInEarlyOutRequest,TotalRejectLateInEarlyOutRequest,
			TotalPendingChangeShiftRequest,TotalApprovedChangeShiftRequest,TotalRejectChangeShiftRequest,
			TotalPendingLeaveRequest,TotalApprovedLeaveRequest,TotalRejectLeaveRequest,
			TotalPendingKIVRequest,TotalApprovedKIVRequest,TotalRejectKIVRequest
			)
SELECT 
	CASE WHEN w.VerifierId = @Empid THEN 'Verifier' ELSE '' END Verifier ,
	CASE WHEN w.ApproverId = @Empid THEN 'Approver' ELSE '' END Approver ,
	-- check in/out request	
	ISNULL(SUM(case when requesttype in (1,2) and  
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				then 1 else 0 end),0) as TotalPendingCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and  
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				then 1 else 0 end),0) as TotalApprovedCheckInOutRequest,
	ISNULL(SUM(case when requesttype in (1,2) and  
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				then 1 else 0 end),0) as TotalRejectCheckInOutRequest,

	-- late in/early out request	
	ISNULL(SUM(case when requesttype in (3,4) and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedLateInEarlyOutRequest,
	ISNULL(SUM(case when requesttype in (3,4) and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectLateInEarlyOutRequest,

	-- Change Shift Request	
	ISNULL(SUM(case when requesttype = 5 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingChangeShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedChangeShiftRequest,
	ISNULL(SUM(case when requesttype = 5 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectChangeShiftRequest,
	
	--Leave Request	
	ISNULL(SUM(case when requesttype = 6 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedLeaveRequest,
	ISNULL(SUM(case when requesttype = 6 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectLeaveRequest,

	--KIV Request	
	ISNULL(SUM(case when requesttype = 7 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalPendingKIVRequest,
	ISNULL(SUM(case when requesttype = 7 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN (2,4,6) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (4,6) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalApprovedKIVRequest,
	ISNULL(SUM(case when requesttype = 7 and    
							CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN(3,5,7) THEN 1
								 ELSE 0 END = 1
				 then 1 else 0 end),0) as TotalRejectKIVRequest
FROM dbo.WorkingTimeRequest w WITH(NOLOCK)
	INNER JOIN 	[dbo].[LookupData] ld WITH(NOLOCK) ON w.RequestType = ld.Value
	INNER JOIN [dbo].[Lookup] l WITH(NOLOCK) ON l.Id = ld.LookupId
WHERE w.CreatedDate >= @fromdate and w.CreatedDate < @toDate
	AND w.EmpId <> @Empid
	AND l.Code = 'RT' 
	AND (w.VerifierId = @Empid OR w.ApproverId = @Empid )
	AND CASE WHEN w.VerifierId = @Empid AND w.CurrentStatus=1 THEN 1
								 WHEN w.ApproverId = @Empid AND w.CurrentStatus IN (1,2) THEN 1
								 ELSE 0 END = 0
GROUP BY
	CASE WHEN w.VerifierId  = @Empid THEN 'Verifier' ELSE '' END,
	CASE WHEN w.ApproverId = @Empid THEN 'Approver' ELSE '' END



-- overtime
	SELECT @TotalOvertimePendingByVerifier = COUNT(1)
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE VerifierId = @Empid
	AND CurrentStatus = 1
	AND (ISNULL(NormalHours, 0) <> 0 OR ISNULL(OffHours, 0) <> 0 OR ISNULL(PublicHours, 0) <> 0)

	SELECT @TotalOvertimePendingByApprover = COUNT(1)
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE ApproverId = @Empid
	AND CurrentStatus = 3
	AND (ISNULL(NormalHours, 0) <> 0 OR ISNULL(OffHours, 0) <> 0 OR ISNULL(PublicHours, 0) <> 0)

	INSERT INTO #Report(RoleId,RoleTitle,
				TotalPendingCheckInOutRequest,TotalApprovedCheckInOutRequest,TotalRejectCheckInOutRequest,
				TotalPendingLateInEarlyOutRequest,TotalApprovedLateInEarlyOutRequest,TotalRejectLateInEarlyOutRequest,
				TotalPendingChangeShiftRequest,TotalApprovedChangeShiftRequest,TotalRejectChangeShiftRequest,
				TotalPendingLeaveRequest,TotalApprovedLeaveRequest,TotalRejectLeaveRequest,
				TotalPendingKIVRequest,TotalApprovedKIVRequest,TotalRejectKIVRequest, TotalPendingOvertime
				)
	
	SELECT  tblrole.RoleId, 'Verifier' RoleTitle,
			SUM(ISNULL(TotalPendingCheckInOutRequest,0)),
			SUM(ISNULL(TotalApprovedCheckInOutRequest,0)),
			SUM(ISNULL(TotalRejectCheckInOutRequest,0)),

			SUM(ISNULL(TotalPendingLateInEarlyOutRequest,0)),
			SUM(ISNULL(TotalApprovedLateInEarlyOutRequest,0)),
			SUM(ISNULL(TotalRejectLateInEarlyOutRequest,0)),

			SUM(ISNULL(TotalPendingChangeShiftRequest,0)),
			SUM(ISNULL(TotalApprovedChangeShiftRequest,0)),
			SUM(ISNULL(TotalRejectChangeShiftRequest,0)),
			
			SUM(ISNULL(TotalPendingLeaveRequest,0)),
			SUM(ISNULL(TotalApprovedLeaveRequest,0)),
			SUM(ISNULL(TotalRejectLeaveRequest,0)),

			SUM(ISNULL(TotalPendingKIVRequest,0)),
			SUM(ISNULL(TotalApprovedKIVRequest,0)),
			SUM(ISNULL(TotalRejectKIVRequest,0)),
			ISNULL(@TotalOvertimePendingByVerifier, 0)
			
	FROM @tblrole tblrole LEFT JOIN #Data Data ON tblrole.Title = Data.Verifier
	Where tblrole.Title = 'Verifier'
	GROUP BY tblrole.RoleId


	INSERT INTO #Report(RoleId,RoleTitle,
				TotalPendingCheckInOutRequest,TotalApprovedCheckInOutRequest,TotalRejectCheckInOutRequest,
				TotalPendingLateInEarlyOutRequest,TotalApprovedLateInEarlyOutRequest,TotalRejectLateInEarlyOutRequest,
				TotalPendingChangeShiftRequest,TotalApprovedChangeShiftRequest,TotalRejectChangeShiftRequest,			
				TotalPendingLeaveRequest,TotalApprovedLeaveRequest,TotalRejectLeaveRequest,
				TotalPendingKIVRequest,TotalApprovedKIVRequest,TotalRejectKIVRequest, TotalPendingOvertime)
	SELECT RoleId,
			'Approver' RoleTitle,
			SUM(ISNULL(TotalPendingCheckInOutRequest,0)),
			SUM(ISNULL(TotalApprovedCheckInOutRequest,0)),
			SUM(ISNULL(TotalRejectCheckInOutRequest,0)),

			SUM(ISNULL(TotalPendingLateInEarlyOutRequest,0)),
			SUM(ISNULL(TotalApprovedLateInEarlyOutRequest,0)),
			SUM(ISNULL(TotalRejectLateInEarlyOutRequest,0)),

			SUM(ISNULL(TotalPendingChangeShiftRequest,0)),
			SUM(ISNULL(TotalApprovedChangeShiftRequest,0)),
			SUM(ISNULL(TotalRejectChangeShiftRequest,0)),
			
			SUM(ISNULL(TotalPendingLeaveRequest,0)),
			SUM(ISNULL(TotalApprovedLeaveRequest,0)),
			SUM(ISNULL(TotalRejectLeaveRequest,0)),

			SUM(ISNULL(TotalPendingKIVRequest,0)),
			SUM(ISNULL(TotalApprovedKIVRequest,0)),
			SUM(ISNULL(TotalRejectKIVRequest,0)),
			ISNULL(@TotalOvertimePendingByApprover, 0)
	FROM @tblrole tblrole LEFT JOIN #Data Data ON tblrole.Title = Data.Approver
	Where tblrole.Title = 'Approver'
	GROUP BY tblrole.RoleId

	SELECT * FROM #Report 

	DROP TABLE #Data 
	DROP TABLE #Report