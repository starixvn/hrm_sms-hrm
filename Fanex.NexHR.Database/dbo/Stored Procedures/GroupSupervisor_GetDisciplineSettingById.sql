
CREATE PROCEDURE [dbo].[GroupSupervisor_GetDisciplineSettingById] @GroupId INT
AS
/*
Created: 20160620@Tim.Trinh
Task: Get list supervisors by group id
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT su.OwnerId
		,e.NickName
		,e.FullName
	FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.OwnerId = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE su.GroupId = @GroupId
		AND su.ActionType = 8 -- edit discipline
		AND su.RequestType = 0
		AND ISNULL(su.IsDeleted, 0) = 0
END
