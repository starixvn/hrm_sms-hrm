

CREATE PROCEDURE [dbo].[Department_GetParent] @DepartmentId INT = NULL
AS
/*
Created: 20151223@Quinn.Vo
Task: Get parent by department id.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblRelatedDepartment (DepartmentId INT)

	INSERT INTO #tblRelatedDepartment (DepartmentId)
	SELECT DepartmentId
	FROM dbo.Department WITH (NOLOCK)
	WHERE ParentId = @DepartmentId

	-- Add new child records until exhausted.
	WHILE @@ROWCOUNT > 0
	BEGIN
		INSERT INTO #tblRelatedDepartment (DepartmentId)
		SELECT dp.DepartmentId
		FROM dbo.Department AS dp WITH (NOLOCK)
			INNER JOIN #tblRelatedDepartment AS d WITH (NOLOCK) ON dp.ParentID = d.DepartmentId
		WHERE NOT EXISTS (
				SELECT 1
				FROM #tblRelatedDepartment AS cur WITH (NOLOCK)
				WHERE cur.DepartmentId = dp.DepartmentId
				)
	END

	SELECT dep.[DepartmentId]
		,dep.[Code]
		,dep.[Name]
		,dep.IsActive
		,dep.[ModifiedDate]
		,dep.[CreatedDate]
		,dep.[ParentId]
		,dep2.[Name]
		,dep.[ManagerId]
		,mg.FullName
		,mg.NickName
	FROM dbo.Department AS dep WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS cr WITH (NOLOCK) ON dep.CreatedBy = cr.EmpId
		LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.EmpId
		LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON dep.[ModifiedBy] = md.EmpId
		LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
	WHERE dep.DepartmentId NOT IN (
			SELECT DepartmentId
			FROM #tblRelatedDepartment WITH (NOLOCK)
			)
		AND dep.DepartmentId <> @DepartmentId
		AND dep.IsActive = 1

	DROP TABLE #tblRelatedDepartment
END
