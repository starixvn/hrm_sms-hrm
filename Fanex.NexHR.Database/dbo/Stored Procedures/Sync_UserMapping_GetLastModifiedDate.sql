
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

	DECLARE @modifiedDate DATETIME

	SELECT @modifiedDate = MAX(UC_ModifiedDate)
	FROM dbo.Employee WITH (NOLOCK)

	SELECT ISNULL(@modifiedDate, '2023-11-10')
END