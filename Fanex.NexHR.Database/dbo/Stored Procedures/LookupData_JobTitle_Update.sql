/****** Object:  Stored Procedure dbo.LookupData_JobTitle_Update    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.LookupData_JobTitle_Update    Script Date: 7/19/2016 5:22:17 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[LookupData_JobTitle_Update]
	@Id INT,
	@Name NVARCHAR(100),
	@CategoryId VARCHAR(500),
	@ErrMsg NVARCHAR(500) = '',
	@ErrCode INT = null    	
AS
/*
Created: Tom.Vo@2013-10-29
Task: Update Job Title
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN

	SET NOCOUNT ON;    

	DECLARE @lookupDataId INT

	SELECT TOP 1 @lookupDataId = ldata.Id
	FROM dbo.LookupData ldata WITH (NOLOCK) 
		INNER JOIN dbo.Lookup lup WITH (NOLOCK) 
			ON ldata.LookupId = lup.Id
	WHERE  lup.Code = 'JT' AND ldata.Name=@Name

	IF (ISNULL(@lookupDataId,@Id) =@Id)
	BEGIN
		UPDATE LookupData 
		SET Name=@Name,
			Value=@CategoryId
		WHERE Id=@Id
		
		SELECT @ErrCode = 0, @ErrMsg = ''
		
		RETURN
	END
	
	SELECT @ErrCode=100,
				@ErrMsg = 'The job title already exist'
		RETURN
END




