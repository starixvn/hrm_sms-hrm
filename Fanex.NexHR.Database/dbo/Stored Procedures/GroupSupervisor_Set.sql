
CREATE PROCEDURE [dbo].[GroupSupervisor_Set] @GroupId INT
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
Created: 20140723@Quinn.Vo
Task: Insert supervisors by role.
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @SupervisorIds TABLE (
		OwnerId INT
		,RequestType TINYINT
		)

	IF ISNULL(@EmpIdCheckInOuts, '') <> ''
	BEGIN
		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,1
		FROM fnc_strSplit(@EmpIdCheckInOuts, ',')

		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,2
		FROM fnc_strSplit(@EmpIdCheckInOuts, ',')
	END

	IF ISNULL(@EmpIdLateInEarlyOuts, '') <> ''
	BEGIN
		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,3
		FROM fnc_strSplit(@EmpIdLateInEarlyOuts, ',')

		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,4
		FROM fnc_strSplit(@EmpIdLateInEarlyOuts, ',')
	END

	IF ISNULL(@EmpIdChangeShifts, '') <> ''
	BEGIN
		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,5
		FROM fnc_strSplit(@EmpIdChangeShifts, ',')
	END

	IF ISNULL(@EmpIdLeaves, '') <> ''
	BEGIN
		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,6
		FROM fnc_strSplit(@EmpIdLeaves, ',')
	END

	IF ISNULL(@EmpIdKeepInViews, '') <> ''
	BEGIN
		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,7
		FROM fnc_strSplit(@EmpIdKeepInViews, ',')
	END

	IF ISNULL(@EmpIdOvertimes, '') <> ''
		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,8
		FROM fnc_strSplit(@EmpIdOvertimes, ',')
		
	IF ISNULL(@EmpIdDisciplines, '') <> ''
	BEGIN
		INSERT INTO @SupervisorIds (
			OwnerId
			,RequestType
			)
		SELECT val
			,0
		FROM fnc_strSplit(@EmpIdDisciplines, ',')
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE su
		SET su.IsDeleted = 1
		,su.ModifiedDate = GETDATE()
		,su.ModifiedBy = @ModifiedBy
		FROM dbo.GroupSupervisor AS su
		WHERE su.GroupId = @GroupId
			AND su.ActionType = @Role
			AND NOT EXISTS (
				SELECT 1
				FROM @SupervisorIds AS sp
				WHERE sp.OwnerId = su.OwnerId
					AND sp.RequestType = su.RequestType
				)

		UPDATE su
		SET su.IsDeleted = 0
		,su.ModifiedDate = GETDATE()
		,su.ModifiedBy = @ModifiedBy
		FROM dbo.GroupSupervisor AS su
		WHERE su.GroupId = @GroupId
			AND su.ActionType = @Role
			AND su.IsDeleted = 1
			AND EXISTS (
				SELECT 1
				FROM @SupervisorIds AS sp
				WHERE sp.OwnerId = su.OwnerId
					AND sp.RequestType = su.RequestType
				)

		DELETE sp
		FROM @SupervisorIds AS sp
		WHERE EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
				WHERE su.OwnerId = sp.OwnerId
					AND su.RequestType = sp.RequestType
					AND su.GroupId = @GroupId
					AND su.ActionType = @Role
				)

		INSERT INTO dbo.GroupSupervisor (
			GroupId
			,ActionType
			,RequestType
			,OwnerId
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			)
		SELECT @GroupId
			,@Role
			,RequestType
			,OwnerId
			,@ModifiedBy
			,GETDATE()
			,0
		FROM @SupervisorIds

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 100
			,@ErrMsg = N'Update supervisors list failed.'
	END CATCH

	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME

	SET @now = getdate()

	--Log general info
	SELECT @LogSPData = '<EmployeeSupervisor_InsertEmpByListOwner ActionType="' + coalesce(cast(@Role AS VARCHAR(50)), space(1)) + '" ' + 'GroupId="' + coalesce(cast(@GroupId AS VARCHAR(100)), space(1)) + '" ' + 'CheckInOuts="' + coalesce(cast(@EmpIdCheckInOuts AS VARCHAR(1000)), space(1)) + '" ' + 'LateInEarlyOuts="' + coalesce(cast(@EmpIdLateInEarlyOuts AS VARCHAR(1000)), space(1)) + '" ' + 'ChangeShifts="' + coalesce(cast(@EmpIdChangeShifts AS VARCHAR(1000)), space(1)) + '" ' + 'Leaves="' + coalesce(cast(@EmpIdLeaves AS VARCHAR(1000)), space(1)) + '" ' + 'KeepInViews="' + coalesce(cast(@EmpIdKeepInViews AS VARCHAR(1000)), space(1)) + '" ' + 'Overtimes="' + coalesce(cast(@EmpIdOvertimes AS VARCHAR(1000)), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 1000
		,@LogDate = @now
		,@Description = 'Update supervisors list'
		,@TrackingValue = @LogSPData
		,@UserID = @ModifiedBy
END