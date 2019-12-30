
CREATE PROCEDURE [dbo].[Auto_AnnualBalance_Update] @Year INT
	,@EmpId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-12-30
Task: Update annual leave balance
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	EXEC dbo.DBA_AnnualLeave_CreateByGroupDate @Year = @Year
		,@EmpId = @EmpId
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	EXEC dbo.DBA_AnnualLeave_CreateAnnualBalance @Year = @Year
		,@EmpId = @EmpId
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT
END
