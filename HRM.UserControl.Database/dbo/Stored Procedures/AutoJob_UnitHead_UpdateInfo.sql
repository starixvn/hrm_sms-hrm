
CREATE PROCEDURE [dbo].[AutoJob_UnitHead_UpdateInfo] @Value NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*  
Created: 20230227@Tim.Trinh
Task: update unit head info
DB:
Original:
Param's Explanation: 
	• @Value:
		<Root>
			<Row SiteId="1" EmpId="1" Code="" Nickname="" FullName="" IsActive="true" IsDeleted="false" DepartmentCode="" DepartmentName="" />
		</Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()

	CREATE TABLE #tempUnitHeadInfo (
		SiteId SMALLINT
		,EmpId INT
		,Code NVARCHAR(20)
		,Nickname NVARCHAR(50)
		,FullName NVARCHAR(100)
		,IsActive BIT
		,IsDeleted BIT
		,DepartmentCode NVARCHAR(50)
		,DepartmentName NVARCHAR(200)
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tempUnitHeadInfo (
		SiteId
		,EmpId
		,Code
		,Nickname
		,FullName
		,IsActive
		,IsDeleted
		,DepartmentCode
		,DepartmentName
		)
	SELECT SiteId
		,EmpId
		,Code
		,Nickname
		,FullName
		,IsActive
		,IsDeleted
		,DepartmentCode
		,DepartmentName
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (SiteId SMALLINT, EmpId INT, Code NVARCHAR(20), Nickname NVARCHAR(50), FullName NVARCHAR(100), IsActive BIT, IsDeleted BIT, DepartmentCode NVARCHAR(50), DepartmentName NVARCHAR(200)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	-- update
	UPDATE uhinfo
	SET uhinfo.Code = tmp.Code
		,uhinfo.Nickname = tmp.Nickname
		,uhinfo.FullName = tmp.FullName
		,uhinfo.IsActive = tmp.IsActive
		,uhinfo.IsDeleted = tmp.IsDeleted
		,uhinfo.DepartmentCode = tmp.DepartmentCode
		,uhinfo.DepartmentName = tmp.DepartmentName
		,uhinfo.SyncDate = @startRun
	FROM dbo.UnitHeadInfo AS uhinfo WITH (NOLOCK)
		INNER JOIN #tempUnitHeadInfo AS tmp WITH (NOLOCK) ON uhinfo.SiteId = tmp.SiteId
		AND uhinfo.EmpId = tmp.EmpId
	WHERE uhinfo.Code != tmp.Code
		OR uhinfo.Nickname != tmp.Nickname
		OR uhinfo.FullName != tmp.FullName
		OR uhinfo.IsActive != tmp.IsActive
		OR uhinfo.IsDeleted != tmp.IsDeleted
		OR uhinfo.DepartmentCode != tmp.DepartmentCode
		OR uhinfo.DepartmentName != tmp.DepartmentName

	INSERT INTO dbo.UnitHeadInfo (
		EmpId
		,SiteId
		,Code
		,Nickname
		,FullName
		,IsActive
		,IsDeleted
		,DepartmentCode
		,DepartmentName
		,SyncDate
		)
	SELECT tmp.EmpId
		,tmp.SiteId
		,tmp.Code
		,tmp.Nickname
		,tmp.FullName
		,tmp.IsActive
		,tmp.IsDeleted
		,tmp.DepartmentCode
		,tmp.DepartmentName
		,@startRun
	FROM #tempUnitHeadInfo AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.UnitHeadInfo AS uhinfo WITH (NOLOCK)
			WHERE uhinfo.EmpId = tmp.EmpId
				AND uhinfo.SiteId = tmp.SiteId
			)

	DROP TABLE #tempUnitHeadInfo
END