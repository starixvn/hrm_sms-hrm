CREATE PROCEDURE [dbo].[Employee_LogEmployeeLogin]
	@EmployeeId INT
	,@LoginTime DATETIME
	,@Platform VARCHAR(255)
	,@IP VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180524@Elton.ton
Task: Log if Employee logins successfully
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[EmployeeLoginLog]
           ([EmployeeId]
           ,[LoginTime]
           ,[Platform]
           ,[IP])
     VALUES
           (@EmployeeId
           ,@LoginTime
           ,@Platform
           ,@IP)
END