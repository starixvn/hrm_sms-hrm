
CREATE PROCEDURE [dbo].[EmployeeSupervisor_InsertEmpByListOwner] @EmpId INT
	,@Role TINYINT
	,@EmpIdLeaves NVARCHAR(MAX) = ''
	,@EmpIdChangeShifts NVARCHAR(MAX) = ''
	,@EmpIdLateInEarlyOuts NVARCHAR(MAX) = ''
	,@EmpIdCheckInOuts NVARCHAR(MAX) = ''
	,@EmpIdOvertimes NVARCHAR(MAX) = ''
	,@EmpIdKeepInViews NVARCHAR(MAX) = ''
	,@EmpIdDisciplines NVARCHAR(MAX) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: @Tom.Vo@2013-10-30
Task: Insert list employee by role (for supervisor list)
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF OBJECT_ID('tempdb..#SupervisorId_multi') IS NOT NULL
		DROP TABLE #SupervisorId_multi

	CREATE TABLE #SupervisorId_multi (
		OwnerId INT
		,RequestType TINYINT
		,OrderNo SMALLINT IDENTITY(1, 1)
		)

	IF ISNULL(@EmpIdCheckInOuts, '') <> ''
	BEGIN
		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,1
		FROM fnc_strSplit(@EmpIdCheckInOuts, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS

		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,2
		FROM fnc_strSplit(@EmpIdCheckInOuts, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS
	END

	IF ISNULL(@EmpIdLateInEarlyOuts, '') <> ''
	BEGIN
		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,3
		FROM fnc_strSplit(@EmpIdLateInEarlyOuts, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS

		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,4
		FROM fnc_strSplit(@EmpIdLateInEarlyOuts, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS
	END

	IF ISNULL(@EmpIdChangeShifts, '') <> ''
	BEGIN
		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,5
		FROM fnc_strSplit(@EmpIdChangeShifts, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS
	END

	IF ISNULL(@EmpIdLeaves, '') <> ''
	BEGIN
		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,6
		FROM fnc_strSplit(@EmpIdLeaves, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS
	END

	IF ISNULL(@EmpIdKeepInViews, '') <> ''
	BEGIN
		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,7
		FROM fnc_strSplit(@EmpIdKeepInViews, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS
	END

	IF ISNULL(@EmpIdOvertimes, '') <> ''
	BEGIN
		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,8
		FROM fnc_strSplit(@EmpIdOvertimes, ',')

		DBCC CHECKIDENT (#SupervisorId_multi, RESEED, 0) WITH NO_INFOMSGS
	END

	IF ISNULL(@EmpIdDisciplines, '') <> ''
	BEGIN
		INSERT INTO #SupervisorId_multi (
			OwnerId
			,RequestType
			)
		SELECT val
			,0
		FROM fnc_strSplit(@EmpIdDisciplines, ',')
	END

	UPDATE sp
	SET sp.IsDeleted = 1
		,sp.ModifiedDate = GETDATE()
		,sp.ModifiedBy = @ModifiedBy
	FROM [dbo].[EmployeeSupervisor] sp
		LEFT JOIN #SupervisorId_multi AS tmpsp ON sp.EmpId = @EmpId
		AND sp.ActionType = @Role
		AND sp.RequestType = tmpsp.RequestType
		AND sp.OwnerId = tmpsp.OwnerId
	WHERE sp.EmpId = @EmpId
		AND sp.ActionType = @Role
		AND tmpsp.OwnerId IS NULL

	UPDATE sp
		SET sp.IsDeleted = 0
		,sp.ModifiedDate = GETDATE()
		,sp.ModifiedBy = @ModifiedBy
		FROM dbo.[EmployeeSupervisor] AS sp
		WHERE sp.EmpId = @EmpId
			AND sp.ActionType = @Role
			AND sp.IsDeleted = 1
			AND EXISTS (
				SELECT 1
				FROM #SupervisorId_multi AS tmpsp
				WHERE sp.OwnerId = tmpsp.OwnerId
					AND sp.RequestType = tmpsp.RequestType
				)

	DELETE tmpsp
	FROM [dbo].[EmployeeSupervisor] sp
		INNER JOIN #SupervisorId_multi AS tmpsp ON sp.EmpId = @EmpId
		AND sp.ActionType = @Role
		AND sp.RequestType = tmpsp.RequestType
		AND sp.OwnerId = tmpsp.OwnerId
	WHERE sp.EmpId = @EmpId

	INSERT INTO [dbo].[EmployeeSupervisor] (
		EmpId
		,OwnerId
		,ActionType
		,RequestType
		,ModifiedBy
		,ModifiedDate
		,OrderNo
		,IsDeleted
		)
	SELECT @EmpId EmpId
		,OwnerId
		,@Role
		,RequestType
		,@ModifiedBy
		,GETDATE()
		,OrderNo
		,0
	FROM #SupervisorId_multi

	SELECT @ErrCode = 0
		,@ErrMsg = ''

	DROP TABLE #SupervisorId_multi

	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME

	SET @now = GETDATE()

	--Log general info
	SELECT @LogSPData = '<EmployeeSupervisor_InsertEmpByListOwner ActionType="' + coalesce(cast(@Role AS VARCHAR(50)), space(1)) + '" ' + 'EmpId="' + coalesce(cast(@EmpId AS VARCHAR(100)), space(1)) + '" ' + 'CheckInOuts="' + coalesce(cast(@EmpIdCheckInOuts AS VARCHAR(1000)), space(1)) + '" ' + 'LateInEarlyOuts="' + coalesce(cast(@EmpIdLateInEarlyOuts AS VARCHAR(1000)), space(1)) + '" ' + 'ChangeShifts="' + coalesce(cast(@EmpIdChangeShifts AS VARCHAR(1000)), space(1)) + '" ' + 'Leaves="' + coalesce(cast(@EmpIdLeaves AS VARCHAR(1000)), space(1)) + '" ' + 'KeepInViews="' + coalesce(cast(@EmpIdKeepInViews AS VARCHAR(1000)), space(1)) + '" ' + 'Overtimes="' + coalesce(cast(@EmpIdOvertimes AS VARCHAR(1000)), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 1000
		,@LogDate = @now
		,@Description = 'Insert Emp By List Owner'
		,@TrackingValue = @LogSPData
		,@UserID = @ModifiedBy
		,@EmpID = @EmpID
END