CREATE PROCEDURE [dbo].[Employee_Information_UpdateByEmpid] @EmpId INT
	,@DOB SMALLDATETIME = NULL
	,@Phone VARCHAR(50) = NULL
	,@Gender TINYINT = NULL
	,@Skype VARCHAR(100) = ''
	,@Yahoo VARCHAR(100) = ''
	,@Marriage TINYINT = 1
	,@PermanentAddress NVARCHAR(250) = ''
	,@CurrentAddress NVARCHAR(250) = ''
	,@IdCardNumber VARCHAR(20) = ''
	,@PassportNo VARCHAR(35) = ''
	,@PassportExp DATETIME = NULL
	,@ChineseName NVARCHAR(200) = NULL
	,@ModifiedBy INT
	,@CountryCode VARCHAR(5) = NULL
	,@PlaceOfbirth NVARCHAR(250) = ''
	,@Ethnic NVARCHAR(250) = ''
	,@Native NVARCHAR(250) = ''
	,@Religion NVARCHAR(250) = ''
	,@DateOfIDCard DATETIME = NULL
	,@PlaceOfIDCard NVARCHAR(250) = ''
	,@HomePhone NVARCHAR(250) = ''
	,@Qualification NVARCHAR(300) = ''
	,@Major NVARCHAR(300) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: TOM
Task: UPDATE an employee Information 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.Employee WITH (ROWLOCK)
		SET DOB = @DOB
			,Phone = @Phone
			,Gender = @Gender
			,Skype = @Skype
			,Yahoo = @Yahoo
			,Marriage = @Marriage
			,ModifiedDate = GETDATE()
			,ModifiedBy = @ModifiedBy
			,PermanentAddress = @PermanentAddress
			,CurrentAddress = @CurrentAddress
			,IdCardNumber = @IdCardNumber
			,PassportNo = @PassportNo
			,PassportExp = @PassportExp
			,ChineseName = @ChineseName
			,PlaceOfbirth = @PlaceOfbirth
			,Ethnic = @Ethnic
			,[Native] = @Native
			,Religion = @Religion
			,DateOfIDCard = @DateOfIDCard
			,PlaceOfIDCard = @PlaceOfIDCard
			,HomePhone = @HomePhone
			,Qualification = @Qualification
			,Major = @Major
		WHERE EmpId = @EmpId

		UPDATE [dbo].[EmployeeSummary] WITH (ROWLOCK)
		SET CountryCode = @CountryCode
		WHERE [EmpId] = @EmpId

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 100
			,@ErrMsg = N'Update Unsuccessfully!';
	END CATCH
END