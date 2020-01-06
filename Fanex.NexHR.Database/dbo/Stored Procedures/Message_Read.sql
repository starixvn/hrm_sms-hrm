
CREATE PROCEDURE [dbo].[Message_Read] @MessageId INT
	,@EmployeeId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140728@Quinn.Vo
Task: Update read message.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.MessageEmployee WITH (NOLOCK)
			WHERE MessageId = @MessageId
				AND EmployeeId = @EmployeeId
			)
	BEGIN
		UPDATE dbo.MessageEmployee WITH (ROWLOCK, UPDLOCK)
		SET IsRead = 1
		WHERE MessageId = @MessageId
			AND EmployeeId = @EmployeeId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.MessageEmployee (
			MessageId
			,EmployeeId
			,Email
			,IsDisabled
			,IsRead
			)
		SELECT @MessageId
			,@EmployeeId
			,Email
			,0
			,1
		FROM dbo.Employee WITH (NOLOCK)
		WHERE EmpId = @EmployeeId
	END
END
