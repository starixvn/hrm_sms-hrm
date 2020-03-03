
CREATE PROCEDURE [dbo].[ChangeShiftRequest_ResetCalendar] @RequestId BIGINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@20200303
Task: reset calendar
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	UPDATE empca
	SET empca.ShiftId = NULL
		,empca.IsOffDay = 1
	FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON empca.EmpId = cs.EmpId
		AND cs.TypeId = 2 -- move off day
		INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
		AND empca.WorkDate = csdt.OnDate

	UPDATE empca
	SET empca.ShiftId = NULL
		,empca.IsOffDay = 1
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON empca.EmpId = cs.EmpId
		AND cs.TypeId = 2 -- move off day
		INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
		AND empca.WorkDate = csdt.OnDate
END
