
CREATE PROCEDURE [dbo].[LeaveRequest_GetDetailById] @RequestId BIGINT
AS
/*
Created: Tim.Trinh@20191014
Task: Get leave request detail by id.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT ldt.RequestId
		,ldt.OnDate
		,ldt.WorkType
		,ldt.LeaveType AS LeaveTypeId
		,lv.[Name] AS LeaveTypeName
		,ldt.IsHalfDay
		,ldt.FromTime
		,ldt.ToTime
	FROM dbo.LeaveRequestDetail AS ldt WITH (NOLOCK)
		INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON ldt.LeaveType = lv.LeaveTypeId
	WHERE ldt.RequestId = @RequestId
END
