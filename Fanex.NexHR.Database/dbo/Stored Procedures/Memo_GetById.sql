/****** Object:  Stored Procedure dbo.Memo_GetById    Script Date: 7/20/2016 10:19:12 AM ******/
/****** Object:  Stored Procedure dbo.Memo_GetById    Script Date: 7/19/2016 5:22:17 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


CREATE PROCEDURE [dbo].[Memo_GetById]
@Id int
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Memo by ID
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
WHERE m.Id = @id

END





