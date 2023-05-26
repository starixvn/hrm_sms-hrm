
CREATE PROCEDURE [dbo].[AutoJob_Employee_ImportFromDB] @SiteId SMALLINT
	,@DbName VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230509@Tim.Trinh
Task: import employee from DB
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @LastModifiedDate DATETIME = '2010-01-01'
		,@SyncDate DATETIME = GETUTCDATE()
		,@sqlExecute NVARCHAR(MAX)

	SELECT @LastModifiedDate = MAX(ModifiedDate)
	FROM dbo.HRM_Employee WITH (NOLOCK)
	WHERE SiteId = @SiteId

	SET @LastModifiedDate = ISNULL(@LastModifiedDate, '2010-01-01')

	CREATE TABLE #tmpEmployee (
		EmpId INT
		,Code NVARCHAR(20)
		,Nickname NVARCHAR(50)
		,FullName NVARCHAR(100)
		,TeamId INT
		,JobTitleId SMALLINT
		,JobLevelId SMALLINT
		,Email VARCHAR(100)
		,IsLocal BIT
		,IsActive BIT
		,IsDeleted BIT
		,JoinedDate DATE
		,ConfirmedDate DATE
		,ResignedDate DATE
		,ModifiedDate DATETIME
		,UserId INT
		)

	SELECT @sqlExecute = 'INSERT INTO #tmpEmployee (
		EmpId
		,UserId
		,Code
		,Nickname
		,FullName
		,TeamId
		,JobTitleId
		,JobLevelId
		,Email
		,IsLocal
		,IsActive
		,IsDeleted
		,JoinedDate
		,ConfirmedDate
		,ResignedDate
		,ModifiedDate)
	SELECT EmployeeId
		,UserId
		,Code
		,Nickname
		,FullName
		,GroupId
		,JobTitle
		,JobLevelId
		,Email
		,IsLocal
		,[Enabled]
		,IsDeleted
		,JoinedDate
		,ConfirmedDate
		,ResignedDate
		,ModifiedDate
	FROM [' + @DbName + '].[dbo].[Employee] WITH (NOLOCK) 
	WHERE ISNULL(IsSpecialUser, 0) = 0 AND ISNULL(ModifiedDate, ''2010-01-01'') > CAST(''' + CONVERT(VARCHAR, @LastModifiedDate, 121) + ''' AS DATETIME)'

	EXEC (@sqlExecute)

	UPDATE emp
	SET emp.UserId = tmp.UserId
		,emp.Code = tmp.Code
		,emp.Nickname = tmp.Nickname
		,emp.FullName = tmp.FullName
		,emp.TeamId = tmp.TeamId
		,emp.JobTitleId = tmp.JobTitleId
		,emp.JobLevelId = tmp.JobLevelId
		,emp.[Email] = tmp.Email
		,emp.IsLocal = ISNULL(tmp.IsLocal, 0)
		,emp.IsActive = ISNULL(tmp.IsActive, 0)
		,emp.IsDeleted = ISNULL(tmp.IsDeleted, 0)
		,emp.JoinedDate = tmp.JoinedDate
		,emp.ConfirmedDate = tmp.ConfirmedDate
		,emp.ResignedDate = tmp.ResignedDate
		,emp.[ModifiedDate] = tmp.ModifiedDate
		,emp.SyncDate = @SyncDate
	FROM dbo.HRM_Employee AS emp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpEmployee AS tmp WITH (NOLOCK) ON emp.EmpId = tmp.EmpId
	WHERE emp.SiteId = @SiteId

	INSERT INTO dbo.HRM_Employee (
		SiteId
		,EmpId
		,UserId
		,Code
		,Nickname
		,FullName
		,TeamId
		,JobTitleId
		,JobLevelId
		,[Email]
		,IsLocal
		,IsActive
		,IsDeleted
		,JoinedDate
		,ConfirmedDate
		,ResignedDate
		,ModifiedDate
		,SyncDate
		)
	SELECT @SiteId
		,tmp.EmpId
		,tmp.UserId
		,tmp.Code
		,tmp.Nickname
		,tmp.FullName
		,tmp.TeamId
		,tmp.JobTitleId
		,tmp.JobLevelId
		,tmp.Email
		,ISNULL(tmp.IsLocal, 0)
		,ISNULL(tmp.IsActive, 0)
		,ISNULL(tmp.IsDeleted, 0)
		,tmp.JoinedDate
		,tmp.ConfirmedDate
		,tmp.ResignedDate
		,tmp.ModifiedDate
		,@SyncDate
	FROM #tmpEmployee AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE emp.SiteId = @SiteId
				AND emp.EmpId = tmp.EmpId
			)

	DROP TABLE #tmpEmployee
END