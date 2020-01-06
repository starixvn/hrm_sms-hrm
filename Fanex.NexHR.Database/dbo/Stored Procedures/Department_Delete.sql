
CREATE PROCEDURE [dbo].[Department_Delete] @DepartmentId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151222@Quinn.Vo
Task: delete Department.
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
			FROM dbo.Department WITH (NOLOCK)
			WHERE [ParentId] = @DepartmentId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The department has child departments'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Department AS dep WITH (NOLOCK)
				INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON dep.DepartmentId = g.DepartmentId
			WHERE dep.DepartmentId = @DepartmentId
				AND g.IsDeleted != 1
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The department has teams'

		RETURN
	END

	UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
	SET IsActive = 0
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE DepartmentId = @DepartmentId
END