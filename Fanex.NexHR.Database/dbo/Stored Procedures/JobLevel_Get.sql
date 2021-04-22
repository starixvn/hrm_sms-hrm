CREATE PROCEDURE [dbo].[JobLevel_Get]
	@IsActive BIT = NULL
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
		,j.Name
		,j.IsActive

	FROM dbo.JobLevel AS j WITH (NOLOCK)
	WHERE j.IsActive = @IsActive OR @IsActive IS NULL

END