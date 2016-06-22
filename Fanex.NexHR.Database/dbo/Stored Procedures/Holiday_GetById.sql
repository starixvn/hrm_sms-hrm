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


