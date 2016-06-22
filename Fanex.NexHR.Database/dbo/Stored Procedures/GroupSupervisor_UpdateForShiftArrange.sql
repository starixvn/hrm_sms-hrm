
CREATE PROCEDURE [dbo].[GroupSupervisor_UpdateForShiftArrange] @GroupId INT
	,@ShiftArrangers VARCHAR(5000) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140723@Quinn.Vo
Task: Insert shift managers for a group.
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Role TINYINT

	SET @Role = 4

	DECLARE @SupervisorIds TABLE (OwnerId INT)

	IF ISNULL(@ShiftArrangers, '') <> ''
	BEGIN
		INSERT INTO @SupervisorIds (OwnerId)
		SELECT val
		FROM fnc_strSplit(@ShiftArrangers, ',')
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE gs
		SET gs.IsDeleted = 1
			,gs.ModifiedDate = GETDATE()
			,gs.ModifiedBy = @ModifiedBy
		FROM dbo.GroupSupervisor AS gs
		WHERE gs.GroupId = @GroupId
			AND gs.ActionType = @Role
			AND NOT EXISTS (
				SELECT 1
				FROM @SupervisorIds AS sp
				WHERE sp.OwnerId = gs.OwnerId
				)

		DELETE gs
		FROM [dbo].GroupSupervisor gs
			INNER JOIN @SupervisorIds AS tmpsp ON gs.GroupId = @GroupId
			AND gs.ActionType = @Role
			AND gs.OwnerId = tmpsp.OwnerId
		WHERE gs.GroupId = @GroupId
			AND ISNULL(gs.IsDeleted, 0) = 1

		DELETE sp
		FROM @SupervisorIds AS sp
		WHERE EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
				WHERE su.OwnerId = sp.OwnerId
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
			)
		SELECT @GroupId
			,@Role
			,0
			,OwnerId
			,@ModifiedBy
			,getdate()
		FROM @SupervisorIds

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 100
			,@ErrMsg = N'Update failed.'
	END CATCH

	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME

	SET @now = getdate()

	--Log general info
	SELECT @LogSPData = '<GroupSupervisor_UpdateForShiftArrange ActionType="' + coalesce(cast(@Role AS VARCHAR(50)), space(1)) + '" ' + 'GroupId="' + coalesce(cast(@GroupId AS VARCHAR(100)), space(1)) + '" ' + 'Owner="' + coalesce(cast(@ShiftArrangers AS VARCHAR(4000)), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 1001
		,@LogDate = @now
		,@Description = 'Insert list Empployee for MngShift'
		,@TrackingValue = @LogSPData
		,@UserID = @ModifiedBy
END
