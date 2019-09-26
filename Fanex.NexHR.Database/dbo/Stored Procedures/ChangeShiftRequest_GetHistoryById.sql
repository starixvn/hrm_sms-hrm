
CREATE PROCEDURE [dbo].[ChangeShiftRequest_GetHistoryById] @RequestId BIGINT
AS
/*
Created: Tim.Trinh@2019-09-24
Task: view request history
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SELECT r.RequestId
		,r.RequestType AS RequestTypeId
		,r.LogDate AS InsertedDate
		,cs.TypeId
		,cs.FromDate
		,cs.ToDate
		,r.Remark
		,r.CurrentStatus
		,r.CreatedDate
		,emp.EmpId
		,emp.NickName
		,emp.FullName
		,gr.GroupId
		,gr.Code
		,gr.[Name]
		,verifier.EmpId AS VerifierId
		,verifier.NickName
		,verifier.FullName
		,approver.EmpId AS ApproverId
		,approver.NickName
		,approver.FullName
	FROM dbo.WorkingTimeRequestHistory AS r WITH (NOLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON r.RequestId = cs.RequestId
		INNER JOIN dbo.Employee AS verifier WITH (NOLOCK) ON verifier.EmpId = r.VerifierId
		INNER JOIN dbo.Employee AS approver WITH (NOLOCK) ON approver.EmpId = r.ApproverId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = r.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE r.RequestId = @RequestId
	ORDER BY r.LogDate DESC
END
