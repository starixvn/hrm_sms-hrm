
CREATE PROCEDURE [dbo].[Auto_Postnatal_UpdateWorkingHour] @EmpId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-12-30
Task: Update working hour per day
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @today DATE = GETDATE()
		,@tomorrow DATE = DATEADD(DAY, 1, GETDATE())

	UPDATE empsum
	SET empsum.WorkingHours = pos.WorkingHours
	FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.EmployeePostnatal AS pos WITH (NOLOCK) ON empsum.EmpId = pos.EmpId
		AND pos.FromDate = @today
		AND empsum.WorkingHours <> pos.WorkingHours
	WHERE @EmpId IS NULL
		OR empsum.EmpId = @EmpId
	
	UPDATE empsum
	SET empsum.WorkingHours = pos.NormalWorkingHours
	FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.EmployeePostnatal AS pos WITH (NOLOCK) ON empsum.EmpId = pos.EmpId
		AND DATEADD(DAY, 1, pos.ToDate) = @tomorrow
		AND empsum.WorkingHours <> pos.NormalWorkingHours
	WHERE @EmpId IS NULL
		OR empsum.EmpId = @EmpId
END
