

CREATE PROCEDURE [dbo].[GroupSupervisor_Get] @TeamIds VARCHAR(MAX) = ''
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

	CREATE TABLE #tblGroup (GroupId INT)

	IF ISNULL(@TeamIds, '') <> ''
	BEGIN
		INSERT INTO #tblGroup (GroupId)
		SELECT val
		FROM fnc_strSplit(@TeamIds, ',')
	END

	SELECT DISTINCT gr.GroupId AS TeamId
		,gr.Code
		,gr.[Name]
		,sec.Code AS Section
		,dep.Code AS Department
	FROM dbo.[Group] AS gr WITH (NOLOCK)
		INNER JOIN #tblGroup AS tblgr WITH (NOLOCK) ON gr.GroupId = tblgr.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON sec.SectionId = gr.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
	WHERE ISNULL(gr.IsDeleted, 0) = 0
	ORDER BY gr.Code

	SELECT DISTINCT su.OwnerId
		,emp.NickName
		,emp.FullName
		,su.ActionType
		,su.GroupId AS TeamId
		,gr.Code
	FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON su.GroupId = gr.GroupId
		AND ISNULL(gr.IsDeleted, 0) = 0
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON su.OwnerId = emp.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
	WHERE su.ActionType IN (2, 3, 4, 5, 8)
		AND ISNULL(su.IsDeleted, 0) = 0

	DROP TABLE #tblGroup
END