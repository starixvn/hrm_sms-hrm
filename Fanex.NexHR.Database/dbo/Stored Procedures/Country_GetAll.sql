CREATE PROCEDURE [dbo].[Country_GetAll]
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
		,CASE 
			WHEN [Nationality] IS NULL THEN [CountryName]
			ELSE [Nationality]
			END [CountryName]
	FROM [dbo].[Country] WITH (NOLOCK)
	--where [Nationality] is not null
	ORDER BY [CountryOrder]
END