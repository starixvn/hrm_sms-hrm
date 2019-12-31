CREATE PROCEDURE [dbo].[Employee_GetEmployees] @query NVARCHAR(256) = NULL
	,@groupIds NVARCHAR(MAX) = ''
	,@enable BIT = NULL
	,@isDelete BIT = NULL
	,@isLocal BIT = NULL
	,@pageNumber INT = 1
	,@pageSize INT = 2000
	,@totalRow INT = NULL OUTPUT
	,@titleId INT = NULL
AS
/*
Created: Tom.Vo@2013-10-23
Task: Insert default working calendar for employee
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #GroupsList (GroupId INT)

	INSERT INTO #GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit(@groupids, ',')

	IF LEN(@query) = 0
		OR @query LIKE '% or %'
		OR @query LIKE '%--%'
	BEGIN
		SET @query = NULL
	END

	SET @query = REPLACE(@query, '''', '''''')

	DECLARE @q NVARCHAR(256) = @query + '%'''

	SELECT @pageNumber = ISNULL(@pageNumber, 1)
		,@pageSize = ISNULL(@pageSize, 2000)

	DECLARE @SQL NVARCHAR(4000)
		,@sWhere NVARCHAR(4000)
		,@bWhere BIT

	SELECT @SQL = 'SELECT @totalRow = count(1) FROM  dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeSummary AS s WITH (NOLOCK) ON s.EmpId = e.EmpId ' + CHAR(13)

	SELECT @sWhere = ''
		,@bWhere = 0

	IF @enable IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + 'WHERE e.isDeleted = 0 and e.Enabled = ' + CONVERT(NVARCHAR(10), @enable) + CHAR(13)
			,@bWhere = 1
	END
	ELSE IF @isDelete IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' e.isDeleted = ' + CONVERT(NVARCHAR(10), @isDelete) + CHAR(13)
			,@bWhere = 1
	END

	IF @q IS NOT NULL
		AND @query IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' (e.NickName LIKE ''' + @q + ' OR e.FullName LIKE N''%' + @q + ' OR e.Code LIKE ''%' + @q + ')' + CHAR(13)
			,@bWhere = 1
	END

	IF ISNULL(@groupids, '') <> ''
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' ( e.GroupId IN (SELECT GroupId FROM #GroupsList) )' + CHAR(13)
			,@bWhere = 1
	END

	IF @titleId IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' JobTitleId = ' + CONVERT(NVARCHAR(10), @titleId) + CHAR(13)
			,@bWhere = 1
	END

	IF @isLocal IS NOT NULL
	BEGIN
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' s.IsLocal = ' + CONVERT(NVARCHAR(10), @isLocal) + CHAR(13)
			,@bWhere = 1
	END

	SELECT @SQL = @SQL + @sWhere

	PRINT @SQL

	EXEC sp_executesql @SQL
		,N'@totalRow INT OUTPUT'
		,@totalRow = @totalRow OUTPUT

	SELECT @SQL = 'SELECT e.[EmpId]
      ,e.[Code]
      ,[DomainUsername]
      ,[FullName]
      ,[NickName]
      ,[DOB]
      ,[Email]
      ,[JobTitleId]
      ,[Enabled]
      ,[Phone]
      ,[Gender]
      ,[JoinedDate]
      ,[ConfirmedDate]
      ,[ResignedDate]
      ,[Skype]
      ,[LetsTalk]
      ,[Marriage]
      ,[LastLogin]
      ,e.[CreatedDate]
      ,e.[CreatedBy]
      ,e.[ModifiedDate]
      ,e.[ModifiedBy]
      ,e.[isDeleted]
      ,[PermanentAddress]
      ,[CurrentAddress]
      ,[IdCardNumber]
      ,[PassportNo]
      ,[PassportExp]
      ,[Password]
      ,[ChineseName]
      ,[PlaceOfbirth]
      ,[Ethnic]
      ,[Native]
      ,[Religion]
      ,[DateOfIDCard]
      ,[PlaceOfIDCard]
      ,[HomePhone]
      ,[Qualification]
      ,[Major]
      ,[PasswordReset]
      ,[EmployeeCard] ' + CHAR(13)
		+ ',s.[AnnualLeaveBalance] ' + CHAR(13)
      	+ ',dbo.fnc_GetKIVBalance(e.EmpId,null) AS KeepInViewBalance ' + CHAR(13)
      	+ ',s.[CanOT] ' + CHAR(13)
      	+ ',s.[IsLocal] ' + CHAR(13)
      	+ ',s.[WorkingDays] ' + CHAR(13)
      	+ ',s.[WorkingHours] ' + CHAR(13)
      	+ ',s.[IsDefaultSchedule] ' + CHAR(13)
      	+ ',s.[CompanyDate] ' + CHAR(13)
      	+ ',s.[IsTopManager] ' + CHAR(13)
      	+ ',s.[OffDaysbyWeek] ' + CHAR(13)
      	+ ',s.CountryCode ' + CHAR(13)
      	+ ',s.IsExpatUseHoliday ' + CHAR(13)
      	+ ',ISNULL(ctry.Nationality,ctry.CountryName) CountryName ' + CHAR(13)
      	+ ',s.Unit ' + CHAR(13)
      	+ ',e_cs.[BankAccount] '+ CHAR(13)
      	+ ',e_cs.[TaxCode] ' + CHAR(13)
      	+ ',e_cs.[SocialInsuranceNo] ' + CHAR(13)
      	+ ',e_cs.[RegisteredHospital] ' + CHAR(13)
      	+ ',e_cs.[Contract1] ' + CHAR(13)
      	+ ',e_cs.[FromDate1] ' + CHAR(13)
      	+ ',e_cs.[ToDate1] ' + CHAR(13)
      	+ ',e_cs.[Contract2] ' + CHAR(13)
      	+ ',e_cs.[FromDate2] ' + CHAR(13)
      	+ ',e_cs.[ToDate2] ' + CHAR(13)
      	+ ',e_cs.[Contract3] ' + CHAR(13)
      	+ ',e_cs.[FromDate3] ' + CHAR(13)
      	+ ',e_cs.[ToDate3] ' + CHAR(13)
      	+ ',e_cs.[Contract4] ' + CHAR(13)
      	+ ',e_cs.[FromDate4] ' + CHAR(13)
      	+ ',e_cs.[ToDate4] ' + CHAR(13)
      	+ ',e_cs.[Contract5] ' + CHAR(13)
      	+ ',e_cs.[FromDate5] ' + CHAR(13)
		+ ',e_cs.[ToDate5] ' + CHAR(13)
		+ ',e_cs.History ' + CHAR(13)
		+ ',lk.Name AS [JobTitle] ' + CHAR(13)
		+ ',g.GroupId AS [TeamID] ' + CHAR(13)
		+ ',g.Code AS [Team] ' + CHAR(13)
		+ ',g.Name AS [TeamName] ' + CHAR(13)
		+ ',g.Rank ' + CHAR(13)
		+ ',(CASE e.EmpId WHEN g.LeaderId THEN 2 ' + CHAR(13)
		+ ' WHEN g.AssistantLeaderId THEN 1' + CHAR(13)
		+ 'ELSE 0 END) AS EmployeeRank ' + CHAR(13)
		+ 'FROM dbo.Employee AS e WITH (NOLOCK) ' + CHAR(13)
		+ 'INNER JOIN dbo.EmployeeSummary AS s WITH (NOLOCK) ON s.EmpId = e.EmpId ' + CHAR(13)
		+ 'INNER JOIN dbo.EmployeeContract AS e_cs WITH (NOLOCK) ON e_cs.EmpId = e.EmpId ' + CHAR(13)
		+ 'LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupID = g.GroupID ' + CHAR(13)
		+ 'LEFT JOIN [dbo].[Country] AS ctry WITH(NOLOCK) ON ctry.CountryCode = s.CountryCode  ' + CHAR(13)
		+ 'LEFT JOIN [dbo].[Lookup] AS l WITH(NOLOCK) ON l.Code = ''JT''  ' + CHAR(13)
		+ 'LEFT JOIN [dbo].[LookupData] AS LK WITH(NOLOCK) ON l.Id = LK.LookupId AND e.JobTitleId = lk.ID ' + CHAR(13)
		+ @sWhere + 'ORDER BY ' + CHAR(13)

	IF (@enable = 0)
	BEGIN
		SELECT @SQL = @SQL + ' e.ResignedDate DESC,'
	END

	SELECT @SQL = @SQL + ' JoinedDate DESC' + CHAR(13)
		+ '  , [Code] DESC' + CHAR(13) +
		'  , [NickName] ' + CHAR(13)
		+ '  , EmployeeRank DESC ' + CHAR(13)
		+ 'OFFSET ' + CONVERT(NVARCHAR(10), (@pageNumber - 1) * @pageSize) + ' ROWS FETCH NEXT ' + CONVERT(NVARCHAR(10), @pageSize) + ' ROWS ONLY;'

	EXEC sp_executesql @SQL

	SELECT emRo.EmployeeId
		,emRo.RoleId
		,loDa.Code
		,loDa.[Name]
	FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
		INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
		INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
		AND look.Code = 'SR' -- System Role

	PRINT @SQL

	DROP TABLE #GroupsList
END
