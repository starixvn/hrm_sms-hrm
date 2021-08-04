
CREATE PROCEDURE [dbo].[EmployeeWorkFromHome_Delete] @EmployeeWorkFromHomeId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210527@Quinn.Vo
Task: Delete Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome WITH (NOLOCK)
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH of this employee does not exists'

		RETURN
	END

	DELETE
	FROM dbo.EmployeeWorkFromHome 
	WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

	SELECT @ErrCode = 0
END