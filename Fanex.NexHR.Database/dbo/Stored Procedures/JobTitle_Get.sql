/****** Object:  Stored Procedure dbo.JobTitle_Get    Script Date: 7/20/2016 10:19:10 AM ******/
/****** Object:  Stored Procedure dbo.JobTitle_Get    Script Date: 7/19/2016 5:22:16 PM ******/

CREATE PROCEDURE [dbo].[JobTitle_Get]
AS
/*
Created: 20150827@Quinn.Vo
Task: Get all Job Title.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT Id
		,LookupId
		,Code
		,NAME
		,Value
	FROM dbo.LookupData WITH (NOLOCK)
	WHERE LookupId = 1
END
