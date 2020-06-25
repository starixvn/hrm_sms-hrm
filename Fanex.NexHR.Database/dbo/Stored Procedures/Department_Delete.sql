
CREATE PROCEDURE [dbo].[Department_Delete] @DepartmentId INT
	,@ModifiedBy INT
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

	IF EXISTS (
			SELECT 1
			FROM dbo.Department AS dep WITH (NOLOCK)
				INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
			WHERE dep.DepartmentId = @DepartmentId AND sec.IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The department has section'

		RETURN
	END

	UPDATE dbo.[Department] WITH (ROWLOCK, UPDLOCK)
	SET IsDeleted = 1
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE DepartmentId = @DepartmentId

END