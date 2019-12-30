
CREATE PROCEDURE [dbo].[Postnatal_Delete] @PostnatalId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Delete Postnatal.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeePostnatal WITH (NOLOCK)
			WHERE PostnatalId = @PostnatalId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee does not exists'

		RETURN
	END

	DECLARE @empId INT
		,@workingHours TINYINT
		,@fromDate DATE
		,@toDate DATE

	SELECT @empId = EmpId
		,@workingHours = NormalWorkingHours
		,@fromDate = FromDate
		,@toDate = ToDate
	FROM dbo.EmployeePostnatal WITH (NOLOCK)
	WHERE PostnatalId = @PostnatalId

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary WITH (NOLOCK)
			WHERE EmpId = @empId
				AND WorkingHours <> @workingHours
			)
	BEGIN
		UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
		SET WorkingHours = @workingHours
		WHERE EmpId = @empId

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @empId
			,@fromDate
			,@toDate
	END

	DELETE
	FROM dbo.EmployeePostnatal
	WHERE PostnatalId = @PostnatalId
END
