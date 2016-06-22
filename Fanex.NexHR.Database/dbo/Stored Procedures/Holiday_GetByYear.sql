---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Holiday_GetByYear]
	@Year INT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Holiday by Year
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
		
	SET NOCOUNT ON;

    SELECT * FROM HOLIDAY WITH (nolock) WHERE YEAR(FromDate) = @Year
	ORDER BY FromDate
END


