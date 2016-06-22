
CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetMemberToAddOtherDiscipine] @SupervisorId INT
	,@Query NVARCHAR(100)
AS
/*
Created: 20160526@Tim.Trinh
Task: Get member by Supervisor.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (ISNULL(@Query, '') <> '')
		SET @Query = '%' + @Query + '%'

	DECLARE @Roles NVARCHAR(100)

	CREATE TABLE #Roles (RoleId INT)

	SELECT @Roles = emp.Roles
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.EmpId = @SupervisorId

	IF ISNULL(@Roles, '') <> ''
	BEGIN
		INSERT INTO #Roles
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@Roles, ',')
	END

	IF EXISTS (
			SELECT 1
			FROM #Roles
			WHERE RoleId IN (6, 7) -- SA/HR
			)
	BEGIN
		SELECT DISTINCT EmpId
			,NickName
			,FullName
		FROM dbo.Employee WITH (NOLOCK)
		WHERE EmpId <> @SupervisorId
			AND (
				NickName LIKE '%' + @query + '%'
				OR FullName LIKE '%' + @query + '%'
				)
			AND isDeleted = 0
			AND [Enabled] = 1
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM #Roles
			WHERE RoleId = 3 -- Approver
			)
	BEGIN
		SELECT DISTINCT emp.EmpId
			,emp.NickName
			,emp.FullName
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON emp.EmpId = es.EmpId
		WHERE es.OwnerId = @SupervisorId
			AND es.EmpId <> @SupervisorId
			AND es.ActionType IN (4, 5)
			AND (
				emp.NickName LIKE '%' + @query + '%'
				OR emp.FullName LIKE '%' + @query + '%'
				)
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
		
		UNION
		SELECT DISTINCT emp.EmpId
			,emp.NickName
			,emp.FullName
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON emp.GroupId = gs.GroupId
		WHERE gs.OwnerId = @SupervisorId
			AND emp.EmpId <> @SupervisorId
			AND gs.ActionType IN (4, 5)
			AND (
				emp.NickName LIKE '%' + @query + '%'
				OR emp.FullName LIKE '%' + @query + '%'
				)
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
	END

	DROP TABLE #Roles
END
