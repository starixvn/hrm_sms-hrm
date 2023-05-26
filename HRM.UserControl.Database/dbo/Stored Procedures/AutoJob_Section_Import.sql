
CREATE PROCEDURE [dbo].[AutoJob_Section_Import] @SiteId SMALLINT
	,@Value NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230316@Tim.Trinh
Task: import section
DB:
Original:

Revisions:
Param's Explanation:
	- @Value: "<Root><Row SectionId='' Code='' Name='' DepartmentId='' IsDeleted='' ModifiedDate='' /></Root>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETUTCDATE()

	CREATE TABLE #tmpSection (
		SectionId INT
		,Code VARCHAR(50)
		,[Name] NVARCHAR(200)
		,DepartmentId INT NULL
		,IsActive BIT NULL
		,IsDeleted BIT NULL
		,ModifiedDate DATETIME NULL
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tmpSection (
		SectionId
		,Code
		,[Name]
		,DepartmentId
		,IsActive
		,IsDeleted
		,ModifiedDate
		)
	SELECT SectionId
		,Code
		,[Name]
		,DepartmentId
		,IsActive
		,IsDeleted
		,CASE 
			WHEN ModifiedDate = '1900-01-01' THEN NULL
			ELSE ModifiedDate
			END
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (SectionId INT, Code VARCHAR(50), [Name] NVARCHAR(200), DepartmentId INT, IsActive BIT, IsDeleted BIT, ModifiedDate DATETIME) AS Data

	EXEC sp_xml_removedocument @DocHandler

	IF EXISTS (
			SELECT 1
			FROM #tmpSection WITH (NOLOCK)
			)
	BEGIN
		INSERT INTO dbo.HRM_Section (
			SiteId
			,SectionId
			,Code
			,[Name]
			,DepartmentId
			,IsActive
			,IsDeleted
			,ModifiedDate
			,SyncDate
			)
		SELECT @SiteId
			,tmp.SectionId
			,tmp.Code
			,tmp.[Name]
			,tmp.DepartmentId
			,tmp.IsActive
			,tmp.IsDeleted
			,tmp.ModifiedDate
			,@SyncDate
		FROM #tmpSection AS tmp WITH (NOLOCK)
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.HRM_Section AS section WITH (NOLOCK)
				WHERE tmp.SectionId = section.SectionId
					AND section.SiteId = @SiteId
				)

		UPDATE section
		SET section.Code = tmp.Code
			,section.[Name] = tmp.[Name]
			,section.DepartmentId = tmp.DepartmentId
			,section.IsActive = ISNULL(tmp.IsActive, 0)
			,section.IsDeleted = ISNULL(tmp.IsDeleted, 0)
			,section.ModifiedDate = tmp.ModifiedDate
			,section.SyncDate = @SyncDate
		FROM dbo.HRM_Section AS section WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tmpSection AS tmp WITH (NOLOCK) ON section.SectionId = tmp.SectionId
		WHERE section.SiteId = @SiteId
	END

	DROP TABLE #tmpSection
END