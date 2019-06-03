
CREATE PROCEDURE [dbo].[Shift_Delete] @ShiftId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140701@Quinn.Vo
Task: Delete a shift 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.[Shift] WITH (NOLOCK)
			WHERE ShiftID = @ShiftId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The shift does not exists '

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE ShiftId = @ShiftId
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.EmployeeAttendance WITH (NOLOCK)
			WHERE ShiftId = @ShiftId
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.ChangeShiftRequestDetail WITH (NOLOCK)
			WHERE FromShift = @ShiftId
				OR ToShift = @ShiftId
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.ScheduleArrangementDefaultShift WITH (NOLOCK)
			WHERE ShiftId = @ShiftId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete shift failed. It has already used another.'

		RETURN
	END

	DELETE
	FROM dbo.[Shift]
	WHERE ShiftID = @ShiftId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete shift failed.'
	END
END
