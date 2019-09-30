
CREATE PROCEDURE [dbo].[ChangeShiftRequest_GetDetailToReviewDiscipline] @RequestId BIGINT
AS
/*
Created: 20190930@Tim.Trinh
Task: Get change shift detail to review discipline
DB: SMS_HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	-- change shift type
	DECLARE @ChangeShift TINYINT = 1
		,@MoveOffDay TINYINT = 2

	SELECT csdt.OnDate
		,csdt.FromShift AS FromShiftId
		,s1.Code AS FromShift
		,csdt.ToShift AS ToShiftId
		,s2.Code AS ToShift
		,csdt.Remark
		,csdt.ToDate
	FROM dbo.ChangeShiftRequest AS cs WITH (NOLOCK)
		INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
		LEFT JOIN dbo.Shift AS s1 WITH (NOLOCK) ON s1.ShiftId = csdt.FromShift
		LEFT JOIN dbo.Shift AS s2 WITH (NOLOCK) ON s2.ShiftId = csdt.ToShift
	WHERE cs.RequestId = @RequestId
		AND cs.TypeId = @ChangeShift
	ORDER BY csdt.OnDate
END
