
CREATE PROCEDURE [dbo].[LeaveRequest_GetByDate] @EmpId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
AS
/*
Created: Quinn.Vo@20190619
Task: Get leave request by empId & date range.
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		lr.RequestId
		,lr.FromDate
		,lr.ToDate
		,lr.TotalDays
		,wk.Remark
		,lr.RefNo
		,wk.CreatedDate
		,wk.CurrentStatus
		,wk.VerifiedDate
		,wk.ApprovedDate
		,wk.HRDate AS ProcessedDate
		,wk.VerifiedStatus
		,wk.ApprovedStatus
		,wk.HRStatus AS ProcessedStatus
		,lr.LeaveType AS LeaveTypeId
		,lv.Name
		,lr.EmpId
		,e.NickName
		,e.FullName
		,ve.EmpId AS VerifierId
		,ve.NickName
		,Ve.FullName
		,ap.EmpId AS ApproverId
		,ap.NickName
		,ap.FullName
		,pr.EmpId AS HRId
		,pr.NickName
		,pr.FullName
	FROM dbo.LeaveRequest AS lr WITH (NOLOCK)
	INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lr.RequestId = wk.RequestId
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lr.EmpId = e.EmpId
	INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON lr.LeaveType = lv.LeaveTypeId
	LEFT JOIN dbo.Employee AS ve WITH (NOLOCK) ON wk.VerifierId = ve.EmpId
	LEFT JOIN dbo.Employee AS ap WITH (NOLOCK) ON wk.ApproverId = ap.EmpId
	LEFT JOIN dbo.Employee AS pr WITH (NOLOCK) ON wk.HRId = pr.EmpId
	WHERE lr.EmpId = @EmpId
		AND (
				(
					lr.FromDate >= @fromdate
					AND lr.FromDate <= @todate
					AND lr.ToDate >= @todate
					)
				OR (
					lr.FromDate <= @fromdate
					AND lr.ToDate >= @todate
					)
				OR (
					lr.ToDate >= @fromdate
					AND lr.ToDate <= @todate
					)
				)
	ORDER BY wk.CreatedDate DESC

	SELECT ldt.RequestId
		,ldt.OnDate
		,ldt.WorkType
		--,ldt.ShiftId
		,ldt.LeaveType AS LeaveTypeId
		,lv.Name AS LeaveTypeName
		,ldt.IsHalfDay
		,ldt.FromTime
		,ldt.FromTime
	FROM dbo.LeaveRequestDetail AS ldt WITH (NOLOCK)
	INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
	INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON ldt.LeaveType = lv.LeaveTypeId
	WHERE lr.EmpId = @EmpId
		AND (
				(
					lr.FromDate >= @fromdate
					AND lr.FromDate <= @todate
					AND lr.ToDate >= @todate
					)
				OR (
					lr.FromDate <= @fromdate
					AND lr.ToDate >= @todate
					)
				OR (
					lr.ToDate >= @fromdate
					AND lr.ToDate <= @todate
					)
				)
END