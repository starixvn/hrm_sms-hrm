
/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/20/2016 10:19:06 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/19/2016 5:22:14 PM ******/
CREATE PROCEDURE [dbo].[EmployeeEmergency_Insert] @EmployeeId INT
	,@FullName NVARCHAR(200)
	,@Phone NVARCHAR(100) = ''
	,@DateOfBirth DATE
	,@Gender BIT
	,@Relationship NVARCHAR(200) = ''
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
		IF (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeEmergency
					WHERE EmpId = @EmployeeId
					)
				)
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'The emergency has already existed. Just allowed add only one emergency';
		END
		ELSE
		BEGIN
			INSERT INTO [dbo].[EmployeeEmergency] (
				[EmpId]
				,FullName
				,Phone
				,DateOfBirth
				,Gender
				,Relationship
				,CreatedBy
				,CreatedDate
				)
			VALUES (
				@EmployeeId
				,@FullName
				,@Phone
				,@DateOfBirth
				,@Gender
				,@Relationship
				,@Creator
				,GETDATE()
				)
		END

		IF @@ERROR <> 0
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Insert relationship failed.';
		END
	END
END