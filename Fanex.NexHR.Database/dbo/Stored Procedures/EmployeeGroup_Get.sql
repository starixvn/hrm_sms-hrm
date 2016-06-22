
CREATE PROCEDURE [dbo].[EmployeeGroup_Get] 
	@FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all groups from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT
	g.GroupId AS TeamId,
	g.DepartmentId,
	g.Code,
	g.Name,
	g.LeaderId,
	g.AssistantLeaderId,
	g.[Rank],
	ISNULL(g.IsDeleted, 0) AS IsDeleted,
	CASE WHEN g.ModifiedDate IS NULL THEN g.CreatedDate ELSE g.ModifiedDate END AS ModifiedDate
	FROM dbo.[Group] AS g WITH (NOLOCK)
	WHERE @FromDate IS NULL
	OR g.ModifiedDate > @FromDate

	ORDER BY g.ModifiedDate
END
