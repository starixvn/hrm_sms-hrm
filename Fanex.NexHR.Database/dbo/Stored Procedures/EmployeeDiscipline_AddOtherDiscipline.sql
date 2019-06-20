

CREATE PROCEDURE [dbo].[EmployeeDiscipline_AddOtherDiscipline] @EmpId INT
	,@WorkDate DATE
	,@DisciplineTypeId SMALLINT
	,@Score DECIMAL(5,2)
	,@ApproverComment NVARCHAR(500) = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160414@Tim.Trinh
Task: Add other violations to employee.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeDiscipline WITH (NOLOCK)
			WHERE WorkDate = @WorkDate
				AND EmpId = @EmpId
				AND DisciplineType = @DisciplineTypeId
				AND DisciplineType IN (- 1, - 2) -- verbal, written warning
				AND ISNULL(IsDeleted, 0) = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This violation has already existed for employee in violation date.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM dbo.EmployeeDiscipline
		WHERE EmpId = @EmpId
			AND DisciplineType = @DisciplineTypeId
			AND WorkDate = @WorkDate
			AND ISNULL(IsDeleted, 1) = 1

		INSERT INTO dbo.EmployeeDiscipline (
			WorkDate
			,EmpId
			,DisciplineType
			,DisciplineScore
			,ApproverComment
			,IsDeleted
			,ModifiedBy
			,ModifiedDate
			)
		VALUES (
			CAST(@WorkDate AS DATE)
			,@EmpId
			,@DisciplineTypeId
			,@Score
			,@ApproverComment
			,0
			,@CreatedBy
			,GETDATE()
			)

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert other violation unsuccessfully.'
	END CATCH
END