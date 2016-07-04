CREATE PROCEDURE [dbo].[GroupSupervisor_Get] @GroupIds VARCHAR(MAX) = ''
AS
/*
Created: 20140723@Quinn.Vo
Task: Get list supervisors by group ids.
DB: SMS-HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #Group (GroupId INT)

	CREATE TABLE #GroupSupervisor (
		GroupId INT
		,OwnerId INT
		,ActionType TINYINT
		)

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT val
		FROM fnc_strSplit(@GroupIds, ',')
	END

	SELECT DISTINCT gr.GroupId AS SectionId
		,gr.Code
		,gr.[Name]
	FROM dbo.[Group] AS gr WITH (NOLOCK)
	WHERE ISNULL(gr.IsDeleted, 0) = 0
		AND (
			ISNULL(@GroupIds, '') = ''
			OR gr.GroupId IN (
				SELECT GroupId
				FROM #Group
				)
			)
	ORDER BY gr.Code

	SELECT DISTINCT su.OwnerId
		,emp.NickName
		,emp.FullName
		,su.ActionType
		,su.GroupId AS SectionId
		,gr.Code
	FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON su.GroupId = gr.GroupId
		AND ISNULL(gr.IsDeleted, 0) = 0
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON su.OwnerId = emp.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
	WHERE su.ActionType IN (2, 3, 4, 5, 8)

	DROP TABLE #Group

	DROP TABLE #GroupSupervisor
END