
CREATE PROCEDURE [dbo].[AutoJob_Employee_Import] @SiteId SMALLINT
	,@Value NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230316@Tim.Trinh
Task: import employee
DB:
Original:

Revisions:
Param's Explanation:
	- @Value: "<Root><Row EmpId='' Code='' Nickname='' FullName='' TeamId='' IsLocal='' IsActive='' IsDeleted=''
		Email='' JoinedDate='' ConfirmedDate='' ResignedDate='' ModifiedDate='' /></Root>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETUTCDATE()

	CREATE TABLE #tmpEmployee (
		EmpId INT
		,Code VARCHAR(20)
		,Nickname NVARCHAR(50)
		,FullName NVARCHAR(100)
		,TeamId INT
		,JobTitleId SMALLINT
		,JobLevelId SMALLINT
		,IsLocal BIT
		,IsActive BIT
		,IsDeleted BIT
		,Email NVARCHAR(100)
		,JoinedDate DATE
		,ConfirmedDate DATE
		,ResignedDate DATE
		,ModifiedDate DATETIME
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tmpEmployee (
		EmpId
		,Code
		,Nickname
		,FullName
		,TeamId
		,JobTitleId
		,JobLevelId
		,IsLocal
		,IsActive
		,IsDeleted
		,Email
		,JoinedDate
		,ConfirmedDate
		,ResignedDate
		,ModifiedDate
		)
	SELECT EmpId
		,Code
		,Nickname
		,FullName
		,TeamId
		,JobTitleId
		,JobLevelId
		,IsLocal
		,IsActive
		,IsDeleted
		,Email
		,CASE 
			WHEN JoinedDate = '1900-01-01' THEN NULL
			ELSE JoinedDate
			END
		,CASE 
			WHEN ConfirmedDate = '1900-01-01' THEN NULL
			ELSE ConfirmedDate
			END
		,CASE 
			WHEN ResignedDate = '1900-01-01' THEN NULL
			ELSE ResignedDate
			END
		,CASE 
			WHEN ModifiedDate = '1900-01-01' THEN NULL
			ELSE ModifiedDate
			END
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmpId INT, Code VARCHAR(20), Nickname NVARCHAR(50), FullName NVARCHAR(100)
		,TeamId INT, JobTitleId SMALLINT, JobLevelId SMALLINT, IsLocal BIT, IsActive BIT, IsDeleted BIT, Email NVARCHAR(100)
		,JoinedDate DATE, ConfirmedDate DATE, ResignedDate DATE, ModifiedDate DATETIME) AS Data

	EXEC sp_xml_removedocument @DocHandler

	INSERT INTO dbo.HRM_Employee (
		SiteId
		,EmpId
		,Code
		,Nickname
		,FullName
		,TeamId
		,JobTitleId
		,JobLevelId
		,IsLocal
		,IsActive
		,IsDeleted
		,[Email]
		,JoinedDate
		,ConfirmedDate
		,ResignedDate
		,ModifiedDate
		,SyncDate
		)
	SELECT @SiteId
		,tmp.EmpId
		,tmp.Code
		,tmp.Nickname
		,tmp.FullName
		,tmp.TeamId
		,tmp.JobTitleId
		,tmp.JobLevelId
		,ISNULL(tmp.IsLocal, 0)
		,ISNULL(tmp.IsActive, 0)
		,ISNULL(tmp.IsDeleted, 0)
		,tmp.Email
		,tmp.JoinedDate
		,tmp.ConfirmedDate
		,tmp.ResignedDate
		,tmp.ModifiedDate
		,@SyncDate
	FROM #tmpEmployee AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE tmp.EmpId = emp.EmpId
				AND emp.SiteId = @SiteId
			)

	UPDATE emp
	SET emp.Nickname = tmp.Nickname
		,emp.FullName = tmp.FullName
		,emp.TeamId = tmp.TeamId
		,emp.JobTitleId = tmp.JobTitleId
		,emp.JobLevelId = tmp.JobLevelId
		,emp.IsLocal = ISNULL(tmp.IsLocal, 0)
		,emp.IsActive = ISNULL(tmp.IsActive, 0)
		,emp.IsDeleted = ISNULL(tmp.IsDeleted, 0)
		,emp.Email = tmp.Email
		,emp.JoinedDate = tmp.JoinedDate
		,emp.ConfirmedDate = tmp.ConfirmedDate
		,emp.ResignedDate = tmp.ResignedDate
		,emp.ModifiedDate = tmp.ModifiedDate
		,emp.SyncDate = @SyncDate
	FROM dbo.HRM_Employee AS emp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpEmployee AS tmp WITH (NOLOCK) ON emp.EmpId = tmp.EmpId
	WHERE emp.SiteId = @SiteId

	DROP TABLE #tmpEmployee
END