
CREATE PROCEDURE [dbo].[Paternity_Delete] @PaternityId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Delete Paternity.
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeePaternity WITH (NOLOCK)
			WHERE PaternityId = @PaternityId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee does not exists'

		RETURN
	END

	DELETE
	FROM dbo.EmployeePaternity
	WHERE PaternityId = @PaternityId
END
