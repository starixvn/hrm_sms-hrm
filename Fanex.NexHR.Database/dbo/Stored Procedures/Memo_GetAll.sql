---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


CREATE PROCEDURE [dbo].[Memo_GetAll]
@content NVARCHAR, 
@year int,
@category int
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Memo
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
SET NOCOUNT ON

SELECT m.*, lk.[Name] as CategoryName 
FROM dbo.MEMO m WITH(NOLOCK) 
	INNER JOIN dbo.LookupData lk WITH(NOLOCK) on lk.id = m.Category
WHERE year(m.SentDate) = @year
AND Enabled = 1
AND ((@content is NULL) OR (m.Subject LIKE '%'+@content+'%' OR m.Contents LIKE '%'+@content+'%'))
AND (@category = -1 OR m.Category = @category)
order by m.SentDate desc

END





