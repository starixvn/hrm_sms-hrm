CREATE PROCEDURE [dbo].[Unit_Delete] @UnitId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 202005/21@Patsy.Doan
Task: delete Unit.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Unit AS u WITH (NOLOCK)
				INNER JOIN dbo.Department AS d WITH (NOLOCK) ON d.UnitId = u.UnitId
			WHERE u.UnitId = @UnitId AND d.IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The unit has department'

		RETURN
	END

	UPDATE dbo.[Unit] WITH (ROWLOCK, UPDLOCK)
	SET IsDeleted = 1
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE UnitId = @UnitId
END



