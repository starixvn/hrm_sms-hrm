
CREATE PROCEDURE [dbo].[EmployeeSupervisor_InsertEmpMngShiftList] @EmpId INT
	,@EmpIdShift VARCHAR(5000) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: @Tom.Vo@2013-10-30
Task: Insert list employee by  Shift role
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Role TINYINT

	SET @Role = 4

	CREATE TABLE #SupervisorId (
		OwnerId INT
		,RequestType TINYINT
		,OrderNo SMALLINT IDENTITY(1, 1)
		)

	IF ISNULL(@EmpIdShift, '') <> ''
		INSERT INTO #SupervisorId (
			OwnerId
			,RequestType
			)
		SELECT val
			,RequestType
		FROM fnc_strSplit(@EmpIdShift, ',')
			,(
				SELECT 0 AS RequestType
				) tblRole

	UPDATE sp
	SET sp.IsDeleted = 1
		,sp.ModifiedDate = GETDATE()
		,sp.ModifiedBy = @ModifiedBy
	FROM [dbo].[EmployeeSupervisor] sp
		LEFT JOIN #SupervisorId AS tmpsp ON sp.EmpId = @EmpId
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
				FROM #SupervisorId AS tmpsp
				WHERE sp.OwnerId = tmpsp.OwnerId
					AND sp.RequestType = tmpsp.RequestType
				)

	DELETE tmpsp
	FROM [dbo].[EmployeeSupervisor] sp
		INNER JOIN #SupervisorId AS tmpsp ON sp.EmpId = @EmpId
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
	FROM #SupervisorId

	SELECT @ErrCode = 0
		,@ErrMsg = ''

	DROP TABLE #SupervisorId

	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME

	SET @now = getdate()

	--Log general info
	SELECT @LogSPData = '<EmployeeSupervisor_InsertEmpMngShiftList ActionType="' + coalesce(cast(@Role AS VARCHAR(50)), space(1)) + '" ' + 'EmpId="' + coalesce(cast(@EmpId AS VARCHAR(100)), space(1)) + '" ' + 'Owner="' + coalesce(cast(@EmpIdShift AS VARCHAR(4000)), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 1001
		,@LogDate = @now
		,@Description = 'Insert list Empployee for MngShift'
		,@TrackingValue = @LogSPData
		,@UserID = @ModifiedBy
		,@EmpID = @EmpID
END