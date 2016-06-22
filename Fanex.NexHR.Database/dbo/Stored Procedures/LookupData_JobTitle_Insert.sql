---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[LookupData_JobTitle_Insert]
	@LookupCode VARCHAR(20),
	@Name NVARCHAR(100),
	@CategoryId VARCHAR(500),
	@ErrMsg NVARCHAR(500) = '',
	@ErrCode INT = null    	
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @lookupDataId INT,@lookupId INT

	SELECT TOP 1 @lookupDataId = ldata.Id , @lookupId = lup.Id
	FROM dbo.Lookup lup WITH (NOLOCK) 
		LEFT JOIN dbo.LookupData ldata WITH (NOLOCK) 
			ON ldata.LookupId = lup.Id AND ldata.Name=@Name
	WHERE  lup.Code = 'JT' 

	PRINT @lookupDataId
	IF (@lookupDataId IS NOT NULL)
	BEGIN
		SELECT @ErrCode=100,
				@ErrMsg = 'The job title already exist'
		RETURN
	END

	INSERT LookupData(LookupId,Code,Name,Value)
	VALUES(@lookupId,NULL,@Name,@CategoryId)

	SELECT @ErrCode = 0, @ErrMsg = ''
END
