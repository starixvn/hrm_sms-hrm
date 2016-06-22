---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Country_GetByName]
	@CountryName NVARCHAR(150)
AS
/*
Created: Tom.Vo@2013-10-21
Task: Get all data on Country BY CountryName
DB: HRMS
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT [CountryCode]
		  ,[Nationality] [CountryName]
	  FROM [dbo].[Country] WITH(NOLOCK)
	  WHERE [Nationality] like @CountryName + '%'
	  ORDER BY [CountryOrder]

END



