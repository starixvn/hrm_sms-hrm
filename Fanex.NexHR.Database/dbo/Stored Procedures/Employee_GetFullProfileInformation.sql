CREATE PROCEDURE [dbo].[Employee_GetFullProfileInformation] @query NVARCHAR(256) = NULL
	, @teamIds NVARCHAR(MAX) = ''
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
	- 20200714@Merlin.Le: Get section and department code
	- 20230704@Otis.Vu: Get supervisor and roles info
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #GroupsList (GroupId INT)
	CREATE CLUSTERED INDEX IX_GroupsList ON #GroupsList (GroupId)

	INSERT INTO #GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit( @teamIds, ',')

	CREATE TABLE #tplEmployee (EmpId INT)
	CREATE CLUSTERED INDEX IX_tplEmployee ON #tplEmployee (EmpId)

	INSERT INTO #tplEmployee (EmpId)
	SELECT e.EmpId
	FROM dbo.Employee e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
	LEFT JOIN #GroupsList g ON g.GroupId = e.GroupId
	WHERE ISNULL(e.isDeleted, 0) = ISNULL(@isDelete, 0)
		AND e.[Enabled] = ISNULL(@enable, 1)
		AND (@titleId IS NULL or e.JobTitleId = @titleId)
		AND (
			ISNULL( @teamIds, '') = ''
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

	SELECT DISTINCT e.EmpId
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
		,g.[Name] AS TeamName
		,sec.Code AS SectionCode
		,dep.Code AS DepartmentCode
		,es.Unit
		,e.EmployeeCard
		,es.CompanyDate
		,e.ConfirmedDate
		,e.ResignedDate
		,e.ResignedReason
		,e.JoinedDate
		,es.MilitaryFromDate
		,es.MilitaryToDate
		,lk.[Name] AS JobTitle
		,jl.[Name] AS JobLevel
		,e.TeamDate
		,e.Ethnic
		,e.[Native]
		,e.Religion
		,ISNULL(ctry.Nationality, ctry.CountryName) AS Nationality
		,e.PassportNo
		,e.PassportExp
		,e.PickupRemark
		,e.Skype
		,e.LetsTalk
		,es.CanOT
		,es.SpecialOT
		,es.WorkingDays
		,es.WorkingHours
		,es.OffDaysbyWeek
		,es.AnnualLeaveBalance
		,es.KeepInViewBalance
		,es.IsTopManager
		,es.IsLocal
	FROM #tplEmployee tpl
	INNER JOIN dbo.Employee e WITH (NOLOCK) ON tpl.EmpId = e.EmpId
	INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
	INNER JOIN dbo.[Section] sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
	INNER JOIN dbo.[Department] dep WITH (NOLOCK) ON sec.DepartmentId = dep.DepartmentId
	LEFT JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON tpl.EmpId = es.EmpId
	LEFT JOIN [dbo].[Lookup] AS l WITH(NOLOCK) ON l.Code = 'JT'
	LEFT JOIN [dbo].[LookupData] AS LK WITH(NOLOCK) ON l.Id = LK.LookupId AND e.JobTitleId = lk.Id
	LEFT JOIN [dbo].[Country] AS ctry WITH(NOLOCK) ON ctry.CountryCode = es.CountryCode
	LEFT JOIN [dbo].[JobLevel] AS jl WITH(NOLOCK) ON jl.JobLevelId = e.JobLevelId AND ISNULL(jl.IsDeleted, 0) = 0
	ORDER BY e.EmpId


	CREATE TABLE #tblSupervisor (EmpId INT, ActionType INT, OwnerId INT, NickName NVARCHAR(20))
	CREATE NONCLUSTERED INDEX IX_tblSupervisor ON #tblSupervisor (EmpId, ActionType, OwnerId, NickName)

	INSERT INTO #tblSupervisor
	SELECT DISTINCT su.EmpId
		,su.ActionType
		,su.OwnerId
		,e.NickName
	FROM dbo.[EmployeeSupervisor] su WITH (NOLOCK)
	INNER JOIN dbo.Employee e WITH (NOLOCK) ON su.OwnerId = e.EmpId
	WHERE su.ActionType IN (2, 3, 4, 5, 8) AND ISNULL(su.IsDeleted, 0) = 0

	SELECT tbl.EmpId, tbl.ActionType, tbl.OwnerId, tbl.NickName
	FROM #tplEmployee tpl WITH (NOLOCK)
	LEFT JOIN #tblSupervisor AS tbl WITH (NOLOCK) ON tpl.EmpId = tbl.EmpId


	SELECT emRo.EmployeeId
		,emRo.RoleId
		,lk.Code
		,lk.[Name]
	FROM #tplEmployee tpl
	LEFT JOIN dbo.EmployeeRole AS emRo WITH (NOLOCK) ON emRo.EmployeeId = tpl.EmpId
	INNER JOIN dbo.LookupData AS lk WITH (NOLOCK) ON emRo.RoleId = lk.Value
	INNER JOIN dbo.[Lookup] AS l WITH (NOLOCK) ON l.Id = lk.LookupId AND l.Code = 'SR' -- System Role


	DROP TABLE #GroupsList

	DROP TABLE #tplEmployee

	DROP TABLE #tblSupervisor
END