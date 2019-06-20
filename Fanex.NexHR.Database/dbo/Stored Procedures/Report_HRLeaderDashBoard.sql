
CREATE PROCEDURE [dbo].[Report_HRLeaderDashBoard] @Empid INT = NULL
	,@date DATE
AS
/*
Created:  Tom.Vo@2013-10-25
Task: show dashboard info for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDate DATE
		,@toDate DATE
		,@TotalOvertimePendingByVerifier INT
		,@TotalOvertimePendingByApprover INT

	SET @date = CONVERT(VARCHAR(10), @date, 111)
	SET @fromDate = dateadd(d, - 7, @date)
	SET @toDate = dateadd(d, 1, @date)

	DECLARE @tblrole TABLE (
		RoleId INT
		,Title NVARCHAR(100)
		)

	INSERT INTO @tblrole
	SELECT er.RoleId
		,loDa.[Name]
	FROM dbo.EmployeeRole AS er WITH (NOLOCK)
		INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON er.RoleId = loDa.Value
		INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
		AND look.Code = 'SR' -- System Role
	WHERE er.EmployeeId = @Empid

	CREATE TABLE #Report (
		RoleId INT
		,RoleTitle NVARCHAR(100)
		,TotalPendingCheckInOutRequest INT
		,TotalApprovedCheckInOutRequest INT
		,TotalRejectCheckInOutRequest INT
		,TotalPendingLateInEarlyOutRequest INT
		,TotalApprovedLateInEarlyOutRequest INT
		,TotalRejectLateInEarlyOutRequest INT
		,TotalPendingChangeShiftRequest INT
		,TotalApprovedChangeShiftRequest INT
		,TotalRejectChangeShiftRequest INT
		,TotalPendingLeaveRequest INT
		,TotalApprovedLeaveRequest INT
		,TotalRejectLeaveRequest INT
		,TotalPendingKIVRequest INT
		,TotalApprovedKIVRequest INT
		,TotalRejectKIVRequest INT
		,TotalPendingOvertime INT
		)

	CREATE TABLE #Data (
		Verifier NVARCHAR(100)
		,Approver NVARCHAR(100)
		,TotalPendingCheckInOutRequest INT
		,TotalApprovedCheckInOutRequest INT
		,TotalRejectCheckInOutRequest INT
		,TotalPendingLateInEarlyOutRequest INT
		,TotalApprovedLateInEarlyOutRequest INT
		,TotalRejectLateInEarlyOutRequest INT
		,TotalPendingChangeShiftRequest INT
		,TotalApprovedChangeShiftRequest INT
		,TotalRejectChangeShiftRequest INT
		,TotalPendingLeaveRequest INT
		,TotalApprovedLeaveRequest INT
		,TotalRejectLeaveRequest INT
		,TotalPendingKIVRequest INT
		,TotalApprovedKIVRequest INT
		,TotalRejectKIVRequest INT
		)

	INSERT INTO #data (
		Verifier
		,Approver
		,TotalPendingCheckInOutRequest
		,TotalApprovedCheckInOutRequest
		,TotalRejectCheckInOutRequest
		,TotalPendingLateInEarlyOutRequest
		,TotalApprovedLateInEarlyOutRequest
		,TotalRejectLateInEarlyOutRequest
		,TotalPendingChangeShiftRequest
		,TotalApprovedChangeShiftRequest
		,TotalRejectChangeShiftRequest
		,TotalPendingLeaveRequest
		,TotalApprovedLeaveRequest
		,TotalRejectLeaveRequest
		,TotalPendingKIVRequest
		,TotalApprovedKIVRequest
		,TotalRejectKIVRequest
		)
	SELECT CASE 
			WHEN w.VerifierId = @Empid THEN 'Verifier'
			ELSE ''
			END Verifier
		,CASE 
			WHEN w.ApproverId = @Empid THEN 'Approver'
			ELSE ''
			END Approver
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (1, 2)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingCheckInOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (1, 2)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedCheckInOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (1, 2)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectCheckInOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (3, 4)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingLateInEarlyOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (3, 4)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedLateInEarlyOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (3, 4)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectLateInEarlyOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 5
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingChangeShiftRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 5
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedChangeShiftRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 5
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectChangeShiftRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 6
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingLeaveRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 6
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedLeaveRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 6
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectLeaveRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 7
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingKIVRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 7
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedKIVRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 7
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectKIVRequest
	FROM dbo.WorkingTimeRequest w WITH (NOLOCK)
		INNER JOIN [dbo].[LookupData] ld WITH (NOLOCK) ON w.RequestType = ld.Value
		INNER JOIN [dbo].[Lookup] l WITH (NOLOCK) ON l.Id = ld.LookupId
	WHERE w.EmpId <> @Empid
		AND l.Code = 'RT'
		AND (
			w.VerifierId = @Empid
			OR w.ApproverId = @Empid
			)
		AND CASE 
			WHEN w.VerifierId = @Empid
				AND w.CurrentStatus = 1 THEN 1
			WHEN w.ApproverId = @Empid
				AND w.CurrentStatus IN (1, 2) THEN 1
			ELSE 0
			END = 1
	GROUP BY CASE 
			WHEN w.VerifierId = @Empid THEN 'Verifier'
			ELSE ''
			END
		,CASE 
			WHEN w.ApproverId = @Empid THEN 'Approver'
			ELSE ''
			END

	INSERT INTO #data (
		Verifier
		,Approver
		,TotalPendingCheckInOutRequest
		,TotalApprovedCheckInOutRequest
		,TotalRejectCheckInOutRequest
		,TotalPendingLateInEarlyOutRequest
		,TotalApprovedLateInEarlyOutRequest
		,TotalRejectLateInEarlyOutRequest
		,TotalPendingChangeShiftRequest
		,TotalApprovedChangeShiftRequest
		,TotalRejectChangeShiftRequest
		,TotalPendingLeaveRequest
		,TotalApprovedLeaveRequest
		,TotalRejectLeaveRequest
		,TotalPendingKIVRequest
		,TotalApprovedKIVRequest
		,TotalRejectKIVRequest
		)
	SELECT CASE 
			WHEN w.VerifierId = @Empid THEN 'Verifier'
			ELSE ''
			END Verifier
		,CASE 
			WHEN w.ApproverId = @Empid THEN 'Approver'
			ELSE ''
			END Approver
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (1, 2)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingCheckInOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (1, 2)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedCheckInOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (1, 2)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectCheckInOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (3, 4)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingLateInEarlyOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (3, 4)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedLateInEarlyOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype IN (3, 4)
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectLateInEarlyOutRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 5
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingChangeShiftRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 5
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedChangeShiftRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 5
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectChangeShiftRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 6
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingLeaveRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 6
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedLeaveRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 6
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectLeaveRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 7
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus = 1 THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (1, 2) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalPendingKIVRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 7
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (2, 4, 6) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (4, 6) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalApprovedKIVRequest
		,ISNULL(SUM(CASE 
					WHEN requesttype = 7
						AND CASE 
							WHEN w.VerifierId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							WHEN w.ApproverId = @Empid
								AND w.CurrentStatus IN (3, 5, 7) THEN 1
							ELSE 0
							END = 1 THEN 1
					ELSE 0
					END), 0) AS TotalRejectKIVRequest
	FROM dbo.WorkingTimeRequest w WITH (NOLOCK)
		INNER JOIN [dbo].[LookupData] ld WITH (NOLOCK) ON w.RequestType = ld.Value
		INNER JOIN [dbo].[Lookup] l WITH (NOLOCK) ON l.Id = ld.LookupId
	WHERE w.CreatedDate >= @fromdate
		AND w.CreatedDate < @toDate
		AND w.EmpId <> @Empid
		AND l.Code = 'RT'
		AND (
			w.VerifierId = @Empid
			OR w.ApproverId = @Empid
			)
		AND CASE 
			WHEN w.VerifierId = @Empid
				AND w.CurrentStatus = 1 THEN 1
			WHEN w.ApproverId = @Empid
				AND w.CurrentStatus IN (1, 2) THEN 1
			ELSE 0
			END = 0
	GROUP BY CASE 
			WHEN w.VerifierId = @Empid THEN 'Verifier'
			ELSE ''
			END
		,CASE 
			WHEN w.ApproverId = @Empid THEN 'Approver'
			ELSE ''
			END

	-- overtime
	SELECT @TotalOvertimePendingByVerifier = COUNT(1)
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE VerifierId = @Empid
		AND CurrentStatus = 1
		AND (
			ISNULL(NormalHours, 0) <> 0
			OR ISNULL(OffHours, 0) <> 0
			OR ISNULL(PublicHours, 0) <> 0
			)

	SELECT @TotalOvertimePendingByApprover = COUNT(1)
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE ApproverId = @Empid
		AND CurrentStatus = 2
		AND (
			ISNULL(NormalHours, 0) <> 0
			OR ISNULL(OffHours, 0) <> 0
			OR ISNULL(PublicHours, 0) <> 0
			)

	INSERT INTO #Report (
		RoleId
		,RoleTitle
		,TotalPendingCheckInOutRequest
		,TotalApprovedCheckInOutRequest
		,TotalRejectCheckInOutRequest
		,TotalPendingLateInEarlyOutRequest
		,TotalApprovedLateInEarlyOutRequest
		,TotalRejectLateInEarlyOutRequest
		,TotalPendingChangeShiftRequest
		,TotalApprovedChangeShiftRequest
		,TotalRejectChangeShiftRequest
		,TotalPendingLeaveRequest
		,TotalApprovedLeaveRequest
		,TotalRejectLeaveRequest
		,TotalPendingKIVRequest
		,TotalApprovedKIVRequest
		,TotalRejectKIVRequest
		,TotalPendingOvertime
		)
	SELECT tblrole.RoleId
		,'Verifier' RoleTitle
		,SUM(ISNULL(TotalPendingCheckInOutRequest, 0))
		,SUM(ISNULL(TotalApprovedCheckInOutRequest, 0))
		,SUM(ISNULL(TotalRejectCheckInOutRequest, 0))
		,SUM(ISNULL(TotalPendingLateInEarlyOutRequest, 0))
		,SUM(ISNULL(TotalApprovedLateInEarlyOutRequest, 0))
		,SUM(ISNULL(TotalRejectLateInEarlyOutRequest, 0))
		,SUM(ISNULL(TotalPendingChangeShiftRequest, 0))
		,SUM(ISNULL(TotalApprovedChangeShiftRequest, 0))
		,SUM(ISNULL(TotalRejectChangeShiftRequest, 0))
		,SUM(ISNULL(TotalPendingLeaveRequest, 0))
		,SUM(ISNULL(TotalApprovedLeaveRequest, 0))
		,SUM(ISNULL(TotalRejectLeaveRequest, 0))
		,SUM(ISNULL(TotalPendingKIVRequest, 0))
		,SUM(ISNULL(TotalApprovedKIVRequest, 0))
		,SUM(ISNULL(TotalRejectKIVRequest, 0))
		,ISNULL(@TotalOvertimePendingByVerifier, 0)
	FROM @tblrole tblrole
		LEFT JOIN #Data Data ON tblrole.Title = Data.Verifier
	WHERE tblrole.Title = 'Verifier'
	GROUP BY tblrole.RoleId

	INSERT INTO #Report (
		RoleId
		,RoleTitle
		,TotalPendingCheckInOutRequest
		,TotalApprovedCheckInOutRequest
		,TotalRejectCheckInOutRequest
		,TotalPendingLateInEarlyOutRequest
		,TotalApprovedLateInEarlyOutRequest
		,TotalRejectLateInEarlyOutRequest
		,TotalPendingChangeShiftRequest
		,TotalApprovedChangeShiftRequest
		,TotalRejectChangeShiftRequest
		,TotalPendingLeaveRequest
		,TotalApprovedLeaveRequest
		,TotalRejectLeaveRequest
		,TotalPendingKIVRequest
		,TotalApprovedKIVRequest
		,TotalRejectKIVRequest
		,TotalPendingOvertime
		)
	SELECT RoleId
		,'Approver' RoleTitle
		,SUM(ISNULL(TotalPendingCheckInOutRequest, 0))
		,SUM(ISNULL(TotalApprovedCheckInOutRequest, 0))
		,SUM(ISNULL(TotalRejectCheckInOutRequest, 0))
		,SUM(ISNULL(TotalPendingLateInEarlyOutRequest, 0))
		,SUM(ISNULL(TotalApprovedLateInEarlyOutRequest, 0))
		,SUM(ISNULL(TotalRejectLateInEarlyOutRequest, 0))
		,SUM(ISNULL(TotalPendingChangeShiftRequest, 0))
		,SUM(ISNULL(TotalApprovedChangeShiftRequest, 0))
		,SUM(ISNULL(TotalRejectChangeShiftRequest, 0))
		,SUM(ISNULL(TotalPendingLeaveRequest, 0))
		,SUM(ISNULL(TotalApprovedLeaveRequest, 0))
		,SUM(ISNULL(TotalRejectLeaveRequest, 0))
		,SUM(ISNULL(TotalPendingKIVRequest, 0))
		,SUM(ISNULL(TotalApprovedKIVRequest, 0))
		,SUM(ISNULL(TotalRejectKIVRequest, 0))
		,ISNULL(@TotalOvertimePendingByApprover, 0)
	FROM @tblrole tblrole
		LEFT JOIN #Data Data ON tblrole.Title = Data.Approver
	WHERE tblrole.Title = 'Approver'
	GROUP BY tblrole.RoleId

	SELECT *
	FROM #Report

	DROP TABLE #Data

	DROP TABLE #Report
END