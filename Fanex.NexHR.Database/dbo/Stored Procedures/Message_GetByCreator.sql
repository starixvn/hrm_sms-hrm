
CREATE PROCEDURE [dbo].[Message_GetByCreator]
	@CreatedBy INT,
	@FromDate DATETIME = NULL,
	@ToDate	DATETIME = NULL,
	@IsDisabled BIT = NULL
AS
/*
Created: 20140714@Quinn.Vo
Task: Get message which created by.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	SET @ToDate = DATEADD(d, 1, @ToDate)

	SELECT 
		ms.MessageId,
		ms.[Subject],
		ms.[Contents],
		ms.[Category],
		ms.[FromDate],
		ms.[ToDate],
		ms.CreatedDate,
		STUFF((
				SELECT CAST(', ' AS VARCHAR(MAX)) + a.Name
				FROM (
					SELECT mg.MessageId, g.Name 
					FROM dbo.[Message] AS m
					INNER JOIN dbo.MessageGroup AS mg WITH (NOLOCK) ON m.MessageId = mg.MessageId
					INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON mg.GroupId = g.GroupId
					WHERE m.CreatedBy = @CreatedBy
					UNION ALL
					SELECT me.MessageId, e.NickName AS Name
					FROM dbo.[Message] AS m
					INNER JOIN dbo.MessageEmployee AS me WITH (NOLOCK) ON m.MessageId = me.MessageId
					INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON me.EmployeeId = e.EmpId
					WHERE m.CreatedBy = @CreatedBy
				) AS a
				WHERE a.MessageId = ms.MessageId
				FOR XML PATH('')
				), 1, 1, '') AS SendTo
	FROM dbo.[Message] AS ms WITH (NOLOCK)
	WHERE ms.CreatedBy = @CreatedBy
	AND (@FromDate IS NULL OR @ToDate IS NULL OR (ms.CreatedDate BETWEEN @FromDate AND @ToDate))
	AND (@IsDisabled IS NULL OR ms.IsDisabled = @IsDisabled)
END
