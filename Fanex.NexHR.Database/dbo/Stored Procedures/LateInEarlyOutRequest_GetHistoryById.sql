
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_GetHistoryById] @RequestId INT
AS
/*
Created: Quinn.Vo@20190626
Task: Get late in/early out request history by RequestId.
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		le.RequestId
		,le.OnDate
		,wk.RequestType AS RequestTypeId
		,le.LateInMins
		,le.EarlyOutMins
		,le.ReasonId
		,wk.Remark
		,le.RefNo
		,CASE WHEN wk.RequestType = 3 THEN att.LateIn ELSE att.EarlyOut END AS ActualLateInEarlyOut
		,sh.Name AS ShiftName
		,wk.CreatedDate
		,wk.CurrentStatus
		,wk.VerifiedDate
		,wk.ApprovedDate
		,wk.HRDate AS ProcessedDate
		,wk.LogDate AS InsertedDate
		,le.EmpId
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
	INNER JOIN dbo.LateInEarlyOutRequestHistory AS le WITH (NOLOCK) ON wk.HistoryId = le.HistoryId
	INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON le.OnDate = cl.WorkDate AND le.EmpId = cl.EmpId
	LEFT JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON cl.ShiftId = sh.ShiftID
	LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON le.EmpId = att.EmpId AND le.OnDate = att.AttendanceDate
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON le.EmpId = e.EmpId
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	LEFT JOIN dbo.Employee AS ve WITH (NOLOCK) ON wk.VerifierId = ve.EmpId
	LEFT JOIN dbo.Employee AS ap WITH (NOLOCK) ON wk.ApproverId = ap.EmpId
	LEFT JOIN dbo.Employee AS pr WITH (NOLOCK) ON wk.HRId = pr.EmpId
	WHERE le.RequestId = @RequestId
	ORDER BY wk.LogDate DESC

END