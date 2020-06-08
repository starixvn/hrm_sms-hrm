CREATE PROCEDURE [dbo].[Department_Delete] @DepartmentId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200521@Lilac.Nguyen
Task: Delete Department
DB: HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Department WITH (NOLOCK)
			WHERE DepartmentId = @DepartmentId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The department does not exists'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Department AS dep WITH (NOLOCK)
				INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
			WHERE dep.DepartmentId = @DepartmentId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The department has section'

		RETURN
	END

	DELETE
	FROM dbo.Department
	WHERE DepartmentId = @DepartmentId

END