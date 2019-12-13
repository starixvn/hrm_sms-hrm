
CREATE PROCEDURE [dbo].[Postnatal_Delete] @PostnatalId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Delete Postnatal.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeePostnatal WITH (NOLOCK)
			WHERE PostnatalId = @PostnatalId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee does not exists'

		RETURN
	END

	DELETE
	FROM dbo.EmployeePostnatal
	WHERE PostnatalId = @PostnatalId
END
