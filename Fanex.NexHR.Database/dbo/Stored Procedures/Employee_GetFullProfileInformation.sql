
CREATE PROCEDURE [dbo].[Employee_GetFullProfileInformation] @query NVARCHAR(256) = NULL
	,@teamIds NVARCHAR(MAX) = ''
	,@enable BIT = NULL
	,@isDelete BIT = NULL
	,@isLocal BIT = NULL
	,@titleId INT = NULL
AS
/*
Created: 20190620@Oliver.Dang
Task: get employee full profile information
DB: HRM
Original:

Revisions:
	20200206MerlinLe: Remove employee emergency
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #GroupsList (GroupId INT)

	INSERT INTO #GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit(@teamIds, ',')

	CREATE TABLE #tplEmployee (EmpId INT)

	INSERT INTO #tplEmployee (EmpId)
	SELECT e.EmpId
	FROM dbo.Employee e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
	LEFT JOIN #GroupsList g ON g.GroupId = e.GroupId
	WHERE ISNULL(e.isDeleted, 0) = ISNULL(@isDelete, 0)
		AND e.[Enabled] = ISNULL(@enable, 1)
		AND (@titleId IS NULL or e.JobTitleId = @titleId)
		AND (
			ISNULL(@teamIds, '') = ''
			OR g.GroupId IS NOT NULL
			)
		AND (
			@isLocal IS NULL
			OR es.IsLocal = @isLocal
			)
		AND (
			ISNULL(@query, '') = ''
			OR e.NickName LIKE '%' + @query + '%'
			OR e.FullName LIKE '%' + @query + '%'
			)

	SELECT e.EmpId
		,e.Code
		,e.FullName
		,e.NickName
		,e.ChineseName
		,e.DOB
		,e.Gender
		,e.Marriage
		,e.CurrentAddress
		,e.PermanentAddress
		,e.IdCardNumber
		,e.DateOfIDCard
		,e.PlaceOfIDCard
		,e.PlaceOfbirth
		,e.Phone
		,e.HomePhone
		,e.Email
		,e.Qualification
		,e.Major
		,g.[Name] AS GroupName
	FROM #tplEmployee tpl
	INNER JOIN dbo.Employee e WITH (NOLOCK) ON tpl.EmpId = e.EmpId
	INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
	ORDER BY e.EmpId

	SELECT r.Id
		,r.EmpId as EmployeeId
		,r.DateOfBirth
		,r.Gender
		,r.FullName
		,r.Phone
	FROM dbo.EmployeeRelationship AS r WITH (NOLOCK)
	INNER JOIN #tplEmployee tpl ON r.EmpId = tpl.EmpId

	DROP TABLE #GroupsList

	DROP TABLE #tplEmployee
END