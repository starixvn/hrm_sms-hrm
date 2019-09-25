
CREATE PROCEDURE [dbo].[ChangeShiftRequest_GetDetailById] @RequestId BIGINT
AS
/*
Created: Tim.Trinh@2019-09-20
Task: get detail by id
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT rd.OnDate
		,rd.FromShift AS FromShiftId
		,s1.Code AS FromShift
		,rd.ToShift AS ToShiftId
		,s2.Code AS ToShift
		,rd.Remark
		,rd.ToDate
	FROM dbo.ChangeShiftRequestDetail AS rd WITH (NOLOCK)
		LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.ShiftID = rd.FromShift
		LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.ShiftID = rd.ToShift
	WHERE rd.RequestID = @RequestId
	ORDER BY rd.OnDate
END
