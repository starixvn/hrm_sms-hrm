
CREATE PROCEDURE [dbo].[EmployeeRole_Update] @EmployeeId INT
	,@RoleIds NVARCHAR(500) = ''
	,@ModifiedBy INT
AS
/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM [dbo].[EmployeeRole]
	WHERE EmployeeId = @EmployeeId

	IF ISNULL(@RoleIds, '') <> ''
	BEGIN
		INSERT INTO [dbo].[EmployeeRole] (
			[EmployeeId]
			,[RoleId]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT DISTINCT @EmployeeId
			,val
			,@ModifiedBy
			,GETDATE()
		FROM fnc_strSplit(@RoleIds, ',')
	END
END
