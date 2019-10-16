
CREATE PROCEDURE [dbo].[EmployeeCalendar_GetShiftByDate] @empId INT
	,@date DATE
AS
/*
Created: Tom.Vo@2013-10-29
Task: get shift of employee on date
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ShiftId INT

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @date
		,@toDate = @date
		,@empIds = @empId

	SELECT @ShiftId = ISNULL(ShiftId, 0)
	FROM dbo.tmpEmployeeCalendar WITH (NOLOCK)
	WHERE EmpId = @empId
		AND WorkDate = @date

	SELECT *
	FROM dbo.[Shift] WITH (NOLOCK)
	WHERE ShiftId = @ShiftId
END
