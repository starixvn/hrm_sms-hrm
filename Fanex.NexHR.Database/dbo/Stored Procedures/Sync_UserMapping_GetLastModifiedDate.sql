
CREATE PROCEDURE [dbo].[Sync_UserMapping_GetLastModifiedDate]
AS
/*
Created: 20221123@Otis.vu
Task: get last user control modifiedDate
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT MAX(UC_ModifiedDate)
	FROM dbo.Employee WITH (NOLOCK)
END