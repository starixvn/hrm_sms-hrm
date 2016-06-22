
CREATE PROCEDURE [dbo].[Message_GetDetailByEmployee] 
	@MessageId INT
AS
/*
Created: 20140729@Quinn.Vo
Task: Get message by id.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		ms.MessageId,
		ms.[Subject],
		ms.[Contents],
		ms.[Category],
		ms.[FromDate],
		ms.[ToDate]
	FROM dbo.[Message] AS ms WITH (NOLOCK)
	WHERE ms.MessageId = @MessageId
END
