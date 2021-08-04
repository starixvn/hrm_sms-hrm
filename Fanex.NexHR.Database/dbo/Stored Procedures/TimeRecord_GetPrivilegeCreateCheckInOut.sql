
CREATE PROCEDURE [dbo].[TimeRecord_GetPrivilegeCreateCheckInOut] @EmpId INT
AS
/*
Created: 20210527@Quinn.Vo
Task: Get Privilege Create Check In/Out Record.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsPrivilegeCreateCheckInOut BIT = 0,
		@Today DATETIME
		
	SET @Today = GETDATE()

	IF (EXISTS(SELECT 1 FROM dbo.EmployeeWorkFromHome WITH (NOLOCK) 
		WHERE EmpId = @EmpId 
			AND @Today > FromDate 
			AND DATEADD (DD, -2, @Today) < ToDate))
	BEGIN
		SET @IsPrivilegeCreateCheckInOut = 1
	END

	SELECT @IsPrivilegeCreateCheckInOut
END