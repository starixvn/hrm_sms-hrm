---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Country_GetByCode]
	@CountryCode  VARCHAR(5)
AS
/*
Created: Tom.Vo@2013-10-21
Task: Get all data on Country
DB: HRMS
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [CountryCode]
		  ,CASE WHEN  [Nationality] is null THEN [CountryName] ELSE [Nationality] END [CountryName]
	  FROM [dbo].[Country] WITH(NOLOCK)
	  WHERE [CountryCode]= @CountryCode


END




