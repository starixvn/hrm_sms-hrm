
CREATE PROCEDURE [dbo].[AutoJob_Team_Import] @SiteId SMALLINT
	,@Value NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230316@Tim.Trinh
Task: import team
DB:
Original:

Revisions:
Param's Explanation:
	- @Value: "<Root><Row TeamId='' Code='' Name='' SectionId='' IsDeleted='' ModifiedDate='' /></Root>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETUTCDATE()

	CREATE TABLE #tmpTeam (
		TeamId INT
		,Code VARCHAR(50)
		,[Name] NVARCHAR(200)
		,SectionId INT NULL
		,IsDeleted BIT NULL
		,ModifiedDate DATETIME NULL
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tmpTeam (
		TeamId
		,Code
		,[Name]
		,SectionId
		,IsDeleted
		,ModifiedDate
		)
	SELECT TeamId
		,Code
		,[Name]
		,SectionId
		,IsDeleted
		,CASE 
			WHEN ModifiedDate = '1900-01-01' THEN NULL
			ELSE ModifiedDate
			END
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (TeamId INT, Code VARCHAR(50), [Name] NVARCHAR(200), SectionId INT, IsDeleted BIT, ModifiedDate DATETIME) AS Data

	EXEC sp_xml_removedocument @DocHandler

	IF EXISTS (
			SELECT 1
			FROM #tmpTeam WITH (NOLOCK)
			)
	BEGIN
		INSERT INTO dbo.HRM_Team (
			SiteId
			,TeamId
			,Code
			,[Name]
			,SectionId
			,IsDeleted
			,ModifiedDate
			,SyncDate
			)
		SELECT @SiteId
			,tmp.TeamId
			,tmp.Code
			,tmp.[Name]
			,tmp.SectionId
			,tmp.IsDeleted
			,tmp.ModifiedDate
			,@SyncDate
		FROM #tmpTeam AS tmp WITH (NOLOCK)
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.HRM_Team AS team WITH (NOLOCK)
				WHERE tmp.TeamId = team.TeamId
					AND team.SiteId = @SiteId
				)

		UPDATE team
		SET team.Code = tmp.Code
			,team.[Name] = tmp.[Name]
			,team.SectionId = tmp.SectionId
			,team.IsDeleted = ISNULL(tmp.IsDeleted, 0)
			,team.ModifiedDate = tmp.ModifiedDate
			,team.SyncDate = @SyncDate
		FROM dbo.HRM_Team AS team WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tmpTeam AS tmp WITH (NOLOCK) ON team.TeamId = tmp.TeamId
		WHERE team.SiteId = @SiteId
	END

	DROP TABLE #tmpTeam
END