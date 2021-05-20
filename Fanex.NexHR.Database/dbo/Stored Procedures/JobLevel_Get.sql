CREATE PROCEDURE [dbo].[JobLevel_Get]
AS
/*
Created: 20210420@Lilac.Nguyen
Task: get joblevel
DB: HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT JobLevelId AS Id
		,[Name]
	FROM dbo.JobLevel WITH (NOLOCK)
	WHERE IsDeleted = 0
END