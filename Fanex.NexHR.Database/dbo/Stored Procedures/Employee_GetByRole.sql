
CREATE PROCEDURE [dbo].[Employee_GetByRole] @RoleIds VARCHAR(20)
	,@query NVARCHAR(256)
	,@ExcludeIds VARCHAR(2000)
AS
/*
Created: 2013/10/22@Quinn
Task: Get list employee by role (for supervisor list)
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @q NVARCHAR(258)

	SET @q = '%' + @query + '%'

	IF LEN(@query) = 0
		OR @query = ';'
	BEGIN
		SET @query = NULL
	END

	CREATE TABLE #ExcludedEmployees (EmpId INT)

	CREATE TABLE #Role (RoleId INT)

	CREATE TABLE #Employee (EmpId INT)

	INSERT INTO #ExcludedEmployees (EmpId)
	SELECT val
	FROM fnc_strSplit(@ExcludeIds, ',')

	INSERT INTO #Role (RoleId)
	SELECT val
	FROM fnc_strSplit(@RoleIds, ',')

	INSERT INTO #Employee
	SELECT e.EmpId AS Id
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeRole AS r WITH (NOLOCK) ON r.EmployeeId = e.EmpId
	WHERE r.RoleId IN (
			SELECT RoleId
			FROM #Role
			)
		AND EmpId NOT IN (
			SELECT EmpId
			FROM #ExcludedEmployees
			)
		AND (
			E.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND e.[Enabled] = 1
		AND (
			@query IS NULL
			OR NickName LIKE @q
			OR FullName LIKE @q
			OR DomainUsername LIKE @q
			OR e.Code LIKE @q
			)

	SELECT e.EmpId AS Id
		,e.Code
		,e.DomainUsername
		,e.FullName
		,e.NickName
		,e.DOB
		,e.Email
		,e.JobTitleId
		,e.[Enabled]
		,e.Phone
		,e.Gender
		,e.ManagerId
		,e.JoinedDate
		,e.ConfirmedDate
		,e.ResignedDate
		,e.Skype
		,e.Yahoo
		,e.Marriage
		,e.LastLogin
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
		,e.PermanentAddress AS [Address]
		,e.CurrentAddress
		,e.IdCardNumber AS CardNo
		,e.PassportNo
		,e.PassportExp
		,e.[Password]
		,e.GroupId
		,ISNULL(e.GroupID, - 1) AS [TeamID]
		,ISNULL(g.[Name], '') AS [Team]
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #Employee e1 ON e1.EmpId = e.EmpId
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId

	SELECT emRo.EmployeeId
		,emRo.RoleId
		,loDa.Code
		,loDa.[Name]
	FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
		INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
		INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
		AND look.Code = 'SR' -- System Role
		INNER JOIN #Employee e ON e.EmpId = emRo.EmployeeId

	DROP TABLE #ExcludedEmployees

	DROP TABLE #Role

	DROP TABLE #Employee
END
