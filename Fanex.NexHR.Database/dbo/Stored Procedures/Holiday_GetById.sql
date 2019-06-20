/****** Object:  Stored Procedure dbo.Holiday_GetById    Script Date: 7/20/2016 10:19:10 AM ******/
/****** Object:  Stored Procedure dbo.Holiday_GetById    Script Date: 7/19/2016 5:22:16 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Holiday_GetById]
	@Id INT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Holiday by id
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM HOLIDAY WITH(nolock) WHERE Id = @Id
END


