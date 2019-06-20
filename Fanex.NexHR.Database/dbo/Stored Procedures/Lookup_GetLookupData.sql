/****** Object:  Stored Procedure dbo.Lookup_GetLookupData    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.Lookup_GetLookupData    Script Date: 7/19/2016 5:22:16 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Lookup_GetLookupData]
	@lookupCode varchar(20)
AS
/*
Created: TOM@2013-10-21
Task: Retrieve data on Lookup Date
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	select l_Data.* from 
		dbo.Lookup l_up WITH(NOLOCK)
		INNER JOIN dbo.LookupData l_Data WITH(NOLOCK)
			ON l_up.Id = l_Data.[LookupId]
		where l_up.Code = @lookupCode
	order by l_Data.[Name]
END




