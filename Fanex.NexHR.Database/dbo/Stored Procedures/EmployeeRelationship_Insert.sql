
/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/20/2016 10:19:06 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/19/2016 5:22:14 PM ******/
CREATE PROCEDURE [dbo].[EmployeeRelationship_Insert] @EmployeeId INT
	,@FullName NVARCHAR(200)
	,@Phone NVARCHAR(100) = ''
	,@DateOfBirth DATE
	,@Gender BIT
	,@Creator INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190611@Oliver.Dang
Task: Insert employee relationship
DB: SPM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	BEGIN
		INSERT INTO [dbo].[EmployeeRelationship] (
			[EmpId]
			,FullName
			,Phone
			,DateOfBirth
			,Gender
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@EmployeeId
			,@FullName
			,@Phone
			,@DateOfBirth
			,@Gender
			,@Creator
			,GETDATE()
			)

		IF @@ERROR <> 0
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Insert relationship failed.';
		END
	END
END