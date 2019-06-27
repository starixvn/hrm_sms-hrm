
CREATE PROCEDURE [dbo].[CheckInOutRequest_GetByDate] @EmpId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
AS
/*
Created: Quinn.Vo@20190624
Task: Get Check In/Out request by empId & date range.
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		mis.RequestId
		,mis.OnDate
		,wk.RequestType AS RequestTypeId
		,mis.RequestTime
		,mis.ReasonId
		,wk.Remark
		,mis.RefNo
		,wk.CreatedDate
		,wk.CurrentStatus
		,wk.VerifiedDate
		,wk.ApprovedDate
		,wk.HRDate AS ProcessedDate
		,mis.EmpId
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
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
	INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON mis.EmpId = e.EmpId
	LEFT JOIN dbo.Employee AS ve WITH (NOLOCK) ON wk.VerifierId = ve.EmpId
	LEFT JOIN dbo.Employee AS ap WITH (NOLOCK) ON wk.ApproverId = ap.EmpId
	LEFT JOIN dbo.Employee AS pr WITH (NOLOCK) ON wk.HRId = pr.EmpId

	WHERE (@EmpId IS NULL OR mis.EmpId = @EmpId)
	AND mis.OnDate BETWEEN @FromDate AND @ToDate

	ORDER BY wk.CreatedDate DESC
END