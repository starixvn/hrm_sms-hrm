
CREATE PROCEDURE [dbo].[AutoJob_Employee_GetLastModifiedDateBySiteId] @SiteId SMALLINT
AS
/*
Created: 20230316@Tim.Trinh
Task: get last modified date
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT MAX(ModifiedDate)
	FROM dbo.HRM_Employee WITH (NOLOCK)
	WHERE SiteId = @SiteId
END