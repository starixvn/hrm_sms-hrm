/****** Object:  Stored Procedure dbo.Country_GetAll    Script Date: 7/20/2016 10:19:00 AM ******/
/****** Object:  Stored Procedure dbo.Country_GetAll    Script Date: 7/19/2016 5:22:11 PM ******/

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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