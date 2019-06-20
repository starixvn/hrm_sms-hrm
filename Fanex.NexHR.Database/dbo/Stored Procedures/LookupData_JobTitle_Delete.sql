/****** Object:  Stored Procedure dbo.LookupData_JobTitle_Delete    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.LookupData_JobTitle_Delete    Script Date: 7/19/2016 5:22:16 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


CREATE PROCEDURE [dbo].[LookupData_JobTitle_Delete]
	@Id int,    
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-29
Task: Delete Job Title
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	IF (EXISTS(SELECT Top 1 1 FROM dbo.Employee e WITH (NOLOCK) 				
			WHERE e.JobTitleId=@Id))
	BEGIN
		SELECT @ErrCode=100,
				@ErrMsg = 'The employee is not empty so cannot delete it'
		RETURN
	END

	DELETE dbo.LookupData	WHERE Id=@Id	

	SELECT @ErrCode = 0, @ErrMsg = ''
END




