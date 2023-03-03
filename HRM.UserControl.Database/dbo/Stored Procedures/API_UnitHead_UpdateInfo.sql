
CREATE PROCEDURE [dbo].[API_UnitHead_UpdateInfo] @SiteId SMALLINT
	,@Value VARCHAR(MAX)
	,@ModifiedBy INT
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
			<Row EmpId="1" Code="" Nickname="" FullName="" IsActive="true" IsDeleted="false" DepartmentCode="" DepartmentName="" />
		</Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData VARCHAR(MAX)
		,@logDataOld VARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tempUnitHeadInfo (
		EmpId INT
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
		EmpId
		,Code
		,Nickname
		,FullName
		,IsActive
		,IsDeleted
		,DepartmentCode
		,DepartmentName
		)
	SELECT EmpId
		,Code
		,Nickname
		,FullName
		,IsActive
		,IsDeleted
		,DepartmentCode
		,DepartmentName
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmpId INT, Code NVARCHAR(20), Nickname NVARCHAR(50), FullName NVARCHAR(100), IsActive BIT, IsDeleted BIT, DepartmentCode NVARCHAR(50), DepartmentName NVARCHAR(200)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	-- write log
	SET @xml = (
			SELECT uhinfo.SiteId AS '@SiteId'
				,uhinfo.EmpId AS '@EmpId'
				,uhinfo.Code AS '@Code'
				,uhinfo.Nickname AS '@Nickname'
				,uhinfo.FullName AS '@FullName'
				,uhinfo.IsActive AS '@IsActive'
				,uhinfo.IsDeleted AS '@IsDeleted'
				,uhinfo.DepartmentCode AS '@DepartmentCode'
				,uhinfo.DepartmentName AS '@DepartmentName'
			FROM dbo.UnitHeadInfo AS uhinfo WITH (NOLOCK)
				INNER JOIN #tempUnitHeadInfo AS tmp WITH (NOLOCK) ON uhinfo.EmpId = tmp.EmpId
			WHERE uhinfo.SiteId = @SiteId
				AND (
					uhinfo.Code != tmp.Code
					OR uhinfo.Nickname != tmp.Nickname
					OR uhinfo.FullName != tmp.FullName
					OR uhinfo.IsActive != tmp.IsActive
					OR uhinfo.IsDeleted != tmp.IsDeleted
					OR uhinfo.DepartmentCode != tmp.DepartmentCode
					OR uhinfo.DepartmentName != tmp.DepartmentName
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

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
		INNER JOIN #tempUnitHeadInfo AS tmp WITH (NOLOCK) ON uhinfo.EmpId = tmp.EmpId
	WHERE uhinfo.SiteId = @SiteId
		AND (
			uhinfo.Code != tmp.Code
			OR uhinfo.Nickname != tmp.Nickname
			OR uhinfo.FullName != tmp.FullName
			OR uhinfo.IsActive != tmp.IsActive
			OR uhinfo.IsDeleted != tmp.IsDeleted
			OR uhinfo.DepartmentCode != tmp.DepartmentCode
			OR uhinfo.DepartmentName != tmp.DepartmentName
			)

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
		,@SiteId
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
				AND uhinfo.SiteId = @SiteId
			)

	-- write log
	SET @xml = (
			SELECT @SiteId AS '@SiteId'
				,EmpId AS '@EmpId'
				,Code AS '@Code'
				,Nickname AS '@Nickname'
				,FullName AS '@FullName'
				,IsActive AS '@IsActive'
				,IsDeleted AS '@IsDeleted'
				,DepartmentCode AS '@DepartmentCode'
				,DepartmentName AS '@DepartmentName'
			FROM #tempUnitHeadInfo AS tmp WITH (NOLOCK)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	--EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
	--	,@SiteId = @SiteId
	--	,@SpName = 'API_UnitHead_UpdateInfo'
	--	,@ObjectId = NULL
	--	,@Data = @logData
	--	,@OldData = @logDataOld
	--	,@LogDate = @startRun
	--	,@Duration = @duration

	DROP TABLE #tempUnitHeadInfo
END