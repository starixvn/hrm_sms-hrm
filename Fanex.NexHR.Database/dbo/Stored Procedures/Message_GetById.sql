/****** Object:  Stored Procedure dbo.Message_GetById    Script Date: 7/20/2016 10:19:13 AM ******/
/****** Object:  Stored Procedure dbo.Message_GetById    Script Date: 7/19/2016 5:22:17 PM ******/

CREATE PROCEDURE [dbo].[Message_GetById] 
	@MessageId INT
AS
/*
Created: 20140717@Quinn.Vo
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
		ms.[ToDate],
		ms.CreatedDate,
		STUFF((
				SELECT ',' +  CAST (mg.GroupId AS VARCHAR(20))
				FROM dbo.MessageGroup AS mg
				WHERE mg.MessageId = ms.MessageId
				FOR XML PATH('')
				), 1, 1, '') AS GroupIds,
		STUFF((
				SELECT  ',' +  CAST (me.EmployeeId AS VARCHAR(20))
				FROM dbo.MessageEmployee AS me
				WHERE me.MessageId = ms.MessageId
				FOR XML PATH('')
				), 1, 1, '') AS EmployeeIds,
		STUFF((
				SELECT CAST(', ' AS VARCHAR(MAX)) + a.Name
				FROM (
					SELECT mg.MessageId, g.Name 
					FROM dbo.[Message] AS m
					INNER JOIN dbo.MessageGroup AS mg WITH (NOLOCK) ON m.MessageId = mg.MessageId
					INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON mg.GroupId = g.GroupId
					WHERE m.MessageId = @MessageId
					UNION ALL
					SELECT me.MessageId, e.NickName AS Name
					FROM dbo.[Message] AS m
					INNER JOIN dbo.MessageEmployee AS me WITH (NOLOCK) ON m.MessageId = me.MessageId
					INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON me.EmployeeId = e.EmpId
					WHERE m.MessageId = @MessageId
				) AS a
				WHERE a.MessageId = ms.MessageId
				FOR XML PATH('')
				), 1, 1, '') AS SendTo
	FROM dbo.[Message] AS ms WITH (NOLOCK)
	WHERE ms.MessageId = @MessageId
END
