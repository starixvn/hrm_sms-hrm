
CREATE PROCEDURE [dbo].[LeaveRequest_GetHistoryById] @RequestId INT
AS
/*
Created: Quinn.Vo@20190617
Task: Get Leave request history by RequestId.
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
		,wk.CreatedDate
		,wk.CurrentStatus
		,wk.VerifiedDate
		,wk.ApprovedDate
		,wk.HRDate AS ProcessedDate
		,wk.LogDate AS InsertedDate
		,lr.LeaveType AS LeaveTypeId
		,lv.Name
		,lr.EmpId
		,e.NickName
		,e.FullName
		,g.GroupId
		,g.Code
		,g.Name
		,ve.EmpId AS VerifierId
		,ve.NickName
		,Ve.FullName
		,ap.EmpId AS ApproverId
		,ap.NickName
		,ap.FullName
		,pr.EmpId AS HRId
		,pr.NickName
		,pr.FullName
	FROM dbo.WorkingTimeRequestHistory AS wk WITH (NOLOCK)
	INNER JOIN dbo.LeaveRequestHistory AS lr WITH (NOLOCK) ON wk.HistoryId = lr.HistoryId
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lr.EmpId = e.EmpId
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON lr.LeaveType = lv.LeaveTypeId
	LEFT JOIN dbo.Employee AS ve WITH (NOLOCK) ON wk.VerifierId = ve.EmpId
	LEFT JOIN dbo.Employee AS ap WITH (NOLOCK) ON wk.ApproverId = ap.EmpId
	LEFT JOIN dbo.Employee AS pr WITH (NOLOCK) ON wk.HRId = pr.EmpId
	WHERE lr.RequestId = @RequestId
	ORDER BY wk.LogDate
END