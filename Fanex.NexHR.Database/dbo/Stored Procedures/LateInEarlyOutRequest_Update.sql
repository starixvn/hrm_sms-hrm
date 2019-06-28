
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_Update] @RequestId BIGINT
	,@EmpId INT
	,@OnDate DATETIME
	,@RequestType SMALLINT
	,@ReasonId TINYINT
	,@LateInMins INT = NULL
	,@EarlyOutMins INT = NULL
	,@Remark NVARCHAR(2000) = NULL
	,@ModifiedBy INT = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190625
Task: Edit a late in/early out request
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @ModifiedDate DATETIME = GETDATE()

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
			WHERE r.RequestId = @RequestId
				AND r.CurrentStatus <> 1
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Process awaiting approval. You could not edit this request.'

		RETURN;
	END

	IF @RequestType NOT IN (3, 4)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'Late in request has the invalid parameter'

		RETURN;
	END

	IF (
			@RequestType IN (3)
			AND (
				@OnDate IS NULL
				OR isnull(@LateInMins, 0) <= 0
				OR isnull(@LateInMins, 0) > 150
				)
			)
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'Late in request has the invalid parameter'

		RETURN;
	END

	IF (
			@RequestType IN (4)
			AND (
				@OnDate IS NULL
				OR isnull(@EarlyOutMins, 0) <= 0
				OR isnull(@EarlyOutMins, 0) > 150
				)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Late out request has the invalid parameter'

		RETURN;
	END

		IF (
			@RequestType IN (3, 4)
			AND EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
					INNER JOIN dbo.LateInEarlyOutRequest AS le WITH (NOLOCK) ON wk.RequestId = le.RequestId
				WHERE wk.empid = @EmpId
					AND wk.RequestId <> wk.RequestId
					AND wk.RequestType = @RequestType
					AND wk.CurrentStatus IN (1, 2, 4, 6)
					AND le.OnDate = @OnDate
				)
			)
	BEGIN
		SET @ErrCode = 105
		SET @ErrMsg = 'There is another request in the same range'

		RETURN;
	END

	DECLARE @IsTopManager BIT

	SELECT @IsTopManager = IsTopManager
	FROM dbo.EmployeeSummary WITH (NOLOCK)
	WHERE EmpId = @EmpId

	IF (
			(
				@ApproverId IS NULL
				OR @VerifierId IS NULL
				)
			AND @IsTopManager = 0
			)
	BEGIN
		SET @ErrCode = 106
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	-- check the late in, ealry out min <=60 
	IF (
			(
				@RequestType = 3
				AND @LateInMins > 60
				)
			OR (
				@RequestType = 4
				AND @EarlyOutMins > 60
				)
			)
	BEGIN
		SET @ErrCode = 108
		SET @ErrMsg = 'The maximun late in/early out minutes is 60'

		RETURN;
	END

	IF @EmpId <> @ModifiedBy
	BEGIN
		IF NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS sp WITH (NOLOCK)
				WHERE sp.OwnerId = @EmpId
					AND sp.EmpId = @ModifiedBy
					AND sp.ActionType = 5
					AND sp.RequestType IN (3, 4)
					AND ISNULL(sp.IsDeleted, 0) = 0
				)
		BEGIN
			SET @ErrCode = 107
			SET @ErrMsg = 'You could not permision on this request.'

			RETURN;
		END
	END

	IF @IsTopManager = 0
	BEGIN
		IF (
				NOT EXISTS (
					SELECT TOP 1 1
					FROM dbo.EmployeeSupervisor AS sp WITH (NOLOCK)
					WHERE sp.OwnerId = @VerifierId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 2
						AND sp.RequestType IN (3, 4)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @VerifierId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (3, 4)
						AND g.ActionType = 2
					)
				)
		BEGIN
			SET @ErrCode = 108
			SET @ErrMsg = 'invalid parameter verifierid.'

			RETURN;
		END

		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS sp WITH (NOLOCK)
					WHERE sp.OwnerId = @ApproverId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 3
						AND sp.RequestType IN (3, 4)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @ApproverId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (3, 4)
						AND g.ActionType = 3
					)
				)
		BEGIN
			SET @ErrCode = 109
			SET @ErrMsg = 'invalid parameter ApproverId.'

			RETURN;
		END
	END

	UPDATE WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
	SET Remark = @Remark
		,CurrentStatus = CASE 
			WHEN @IsTopManager = 1
				AND @VerifierId IS NOT NULL THEN 1
			WHEN @IsTopManager = 1
				AND @ApproverId IS NOT NULL THEN 2
			WHEN @IsTopManager = 1
				AND @ApproverId IS NULL THEN 4
			ELSE 1
			END
		,VerifierId = @VerifierId
		,VerifiedStatus = CASE 
			WHEN @IsTopManager = 1
				AND @VerifierId IS NULL THEN 2
			ELSE 1
			END
		,ApproverId = @ApproverId
		,ApprovedStatus = CASE 
			WHEN @IsTopManager = 1
				AND @ApproverId IS NULL THEN 2
			ELSE 1
			END
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @ModifiedDate
	WHERE RequestId = @RequestId

	UPDATE dbo.LateInEarlyOutRequest WITH (ROWLOCK, UPDLOCK)
	SET [EmpId] = @EmpId
		,[LateInMins] = @LateInMins
		,[EarlyOutMins] = @EarlyOutMins
		,[OnDate] = @OnDate
		,[ReasonId] = @ReasonId
	WHERE [RequestId] = @RequestId

	IF (
			@ApproverId IS NULL
			AND @VerifierId IS NULL
			AND @IsTopManager = 1
			)
	BEGIN
		EXECUTE dbo.LateInEarlyOutRequest_Approve @RequestID = @RequestID
				,@EmpID = @EmpID
				,@ProcessedBy = @EmpID
				,@ProcessedDate = @ModifiedDate
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
	END

	EXECUTE dbo.LateInEarlyOutRequest_WriteLog @RequestId = @RequestId
	
	SELECT @ErrCode = 0
		,@ErrMsg = ''
END