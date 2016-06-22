
CREATE PROCEDURE [dbo].[EmployeeDiscipline_AddOtherDiscipline] @EmpId INT
	,@WorkDate DATETIME
	,@DisciplineTypeId INT
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

	DECLARE @Roles NVARCHAR(100)

	CREATE TABLE #Roles (RoleId INT)

	SELECT @Roles = Roles
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @CreatedBy

	IF ISNULL(@Roles, '') <> ''
	BEGIN
		INSERT INTO #Roles
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@Roles, ',')
	END

	IF NOT EXISTS (
			SELECT 1
			FROM #Roles
			WHERE RoleId = 7 -- SA
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'You do not permission.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeDiscipline WITH (NOLOCK)
			WHERE CAST(WorkDate AS DATE) = CAST(@WorkDate AS DATE)
				AND EmpId = @EmpId
				AND DisciplineType = @DisciplineTypeId
				AND DisciplineType IN (- 1, - 2) -- verbal, written warning
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This violation has already existed for employee in violation date.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO dbo.EmployeeDiscipline (
			WorkDate
			,EmpId
			,DisciplineType
			,ApproverComment
			,ModifiedBy
			,ModifiedDate
			)
		VALUES (
			CAST(@WorkDate AS DATE)
			,@EmpId
			,@DisciplineTypeId
			,@ApproverComment
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

	DROP TABLE #Roles
END
