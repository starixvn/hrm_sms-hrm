/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/20/2016 10:19:06 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/19/2016 5:22:14 PM ******/
CREATE PROCEDURE [dbo].[EmployeeRole_Insert] 
	@EmployeeId INT
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

	IF ISNULL(@RoleIds,'')<>''
	BEGIN
		INSERT INTO [dbo].[EmployeeRole] (
			[EmployeeId]
			,[RoleId]
			,[ModifiedBy]
			,[ModifiedDate] )
		SELECT DISTINCT 
			@EmployeeId
			,val
			,@ModifiedBy
			,GETDATE()
		FROM fnc_strSplit (@RoleIds, ',')
	END
END