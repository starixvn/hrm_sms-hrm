/****** Object:  Stored Procedure dbo.Employee_UpdatePasswordResetStatusByEmpId    Script Date: 7/20/2016 10:19:03 AM ******/
/****** Object:  Stored Procedure dbo.Employee_UpdatePasswordResetStatusByEmpId    Script Date: 7/19/2016 5:22:13 PM ******/

CREATE PROCEDURE [dbo].[Employee_SelfUpdateInfo] 
	@EmpId int,
	@ChineseName NVARCHAR(200) ='',
	@Email VARCHAR(50) = '',
	@Phone VARCHAR(50) = '',
	@Skype VARCHAR(100) = '',
	@Yahoo VARCHAR(100) = '',
	@Address NVARCHAR(250) ='',
	@CurrentAddress NVARCHAR(250) = '',
	@FilePath NVARCHAR(4000) = '',
	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT
AS
/*
Created: 20190517@Oliver.Dang
Task: Employee self update their contact info
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE Employee WITH (UPDLOCK,ROWLOCK)
	SET ChineseName = @ChineseName
	   ,Email = @Email
	   ,Phone = @Phone
	   ,Skype = @Skype
	   ,Yahoo = @Yahoo
	   ,PermanentAddress = @Address
	   ,CurrentAddress = @CurrentAddress
	WHERE empId = @empId

	IF ISNULL(@FilePath, '') <> ''
	BEGIN
		CREATE TABLE #tblFileUpload (fileUpload nvarchar(4000))

		INSERT INTO #tblFileUpload
		SELECT val
		FROM fnc_strSplit(@FilePath, '|')

		DELETE FROM dbo.EmployeeUploadFile
		WHERE EmpId = @EmpId

		INSERT INTO EmployeeUploadFile (EmpId, [FileName], UploadedDate)
		SELECT @EmpId
			  ,fileUpload
			  ,GETDATE()
		FROM #tblFileUpload
		
		DROP TABLE #tblFileUpload 
	END
	IF @@ERROR <> 0
		SELECT @ErrCode = 102, @ErrMsg = N'Update information failed.'
	ELSE 
		SELECT @ErrCode=0,@ErrMsg='Update information successful!'	
END