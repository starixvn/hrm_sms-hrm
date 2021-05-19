CREATE PROCEDURE [dbo].[JobLevel_Get]
AS
/*
Created: 20210420@Lilac.Nguyen
Task: get joblevel by active
DB: HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT j.JobLevelId AS Id
		,j.[Name]
	FROM dbo.JobLevel AS j WITH (NOLOCK)
	WHERE j.IsDeleted = 0

END