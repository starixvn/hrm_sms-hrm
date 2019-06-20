/****** Object:  Stored Procedure dbo.Memo_GetAll    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.Memo_GetAll    Script Date: 7/19/2016 5:22:17 PM ******/
CREATE PROCEDURE [dbo].[Memo_GetAll]
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
WHERE (@year = 0 OR year(m.SentDate) = @year)
AND Enabled = 1
AND (@category = -1 OR m.Category = @category)
order by m.SentDate desc

END





