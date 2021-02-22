
CREATE PROCEDURE [dbo].[KeepInViewRequest_GetHistoryById] @RequestId INT
AS
/*
Created: Quinn.Vo@20190606
Task: Get Keep In View request history by RequestId.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT kiv.RequestId
		,kiv.KeepInViewType AS KeepInViewTypeId
		,kiv.OnDate
		,kiv.FromTime
		,kiv.ToTime
		,kiv.IsHalfDay
		,wk.Remark
		,wk.CurrentStatus
		,wk.VerifiedDate
		,wk.ApprovedDate
		,wk.HRDate AS ProcessedDate
		,wk.LogDate AS InsertedDate
		,kiv.EmpId
		,e.NickName
		,e.FullName
		,g.[Name] AS Team
		,ve.EmpId AS VerifierId
		,ve.NickName
		,Ve.FullName
		,ap.EmpId AS ApproverId
		,ap.NickName
		,ap.FullName
		,pr.EmpId AS HRId
		,pr.NickName
		,pr.FullName
		,m.EmpId AS ModifierId
		,m.NickName
		,m.FullName
		,c.EmpId AS CreaterId
		,c.NickName
		,c.FullName
	FROM dbo.WorkingTimeRequestHistory AS wk WITH (NOLOCK)
		INNER JOIN dbo.KeepInViewRequestHistory AS kiv WITH (NOLOCK) ON wk.HistoryId = kiv.HistoryId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON kiv.EmpId = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.Employee AS ve WITH (NOLOCK) ON wk.VerifierId = ve.EmpId
		LEFT JOIN dbo.Employee AS ap WITH (NOLOCK) ON wk.ApproverId = ap.EmpId
		LEFT JOIN dbo.Employee AS pr WITH (NOLOCK) ON wk.HRId = pr.EmpId
		LEFT JOIN dbo.Employee AS m WITH (NOLOCK) ON wk.ModifiedBy = m.EmpId
		LEFT JOIN dbo.Employee AS c WITH (NOLOCK) ON c.EmpId = (SELECT TOP 1 CreatedBy FROM WorkingTimeRequestHistory WITH (NOLOCK) WHERE RequestId = @RequestId ORDER BY LogDate ASC)
	WHERE kiv.RequestId = @RequestId
	ORDER BY wk.LogDate DESC
END
