CREATE PROCEDURE [dbo].[Unit_Delete] @UnitId INT
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

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Unit WITH (NOLOCK)
			WHERE UnitId = @UnitId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The department does not exists'

		RETURN
	END

	--IF EXISTS (
	--		SELECT 1
	--		FROM dbo.Unit AS u WITH (NOLOCK)
	--			INNER JOIN dbo.Department AS d WITH (NOLOCK) ON d.UnitId = u.UnitId
	--		WHERE u.UnitId = @UnitId
	--		)
	--BEGIN
	--	SELECT @ErrCode = 1
	--		,@ErrMsg = N'The unit has department'

	--	RETURN
	--END

	DELETE dbo.Unit WITH (ROWLOCK, UPDLOCK)
	WHERE UnitId = @UnitId
END



