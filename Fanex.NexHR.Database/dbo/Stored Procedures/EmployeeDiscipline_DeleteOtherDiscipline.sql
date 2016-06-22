
CREATE PROCEDURE [dbo].[EmployeeDiscipline_DeleteOtherDiscipline] @EmpId INT
	,@WorkDate DATETIME
	,@DisciplineTypeId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160425@Tim.Trinh
Task: delete other violations.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM dbo.EmployeeDiscipline
		WHERE EmpId = @EmpId
			AND DisciplineType = @DisciplineTypeId
			AND WorkDate = @WorkDate

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete other violation unsuccessfully.'
	END CATCH
END
