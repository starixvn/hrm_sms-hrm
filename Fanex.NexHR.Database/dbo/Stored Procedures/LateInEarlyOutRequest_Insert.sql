
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_Insert] @EmpId INT
	,@OnDate DATETIME
	,@RequestType SMALLINT
	,@ReasonId TINYINT
	,@LateInMins INT = NULL
	,@EarlyOutMins INT = NULL
	,@Remark NVARCHAR(2000) = NULL
	,@CreatedBy INT = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@RequestId BIGINT = 0 OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190625
Task: Create a late in/early out request
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CreatedDate DATETIME = GETDATE()

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
				OR isnull(@LateInMins, 0) > 60
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
				OR isnull(@EarlyOutMins, 0) > 60
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

	IF @EmpId <> @CreatedBy
	BEGIN
		IF (NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor sp WITH (NOLOCK)
				WHERE sp.OwnerId = @EmpId
					AND sp.EmpId = @CreatedBy
					AND sp.ActionType = 5
					AND sp.RequestType IN (3, 4)
					AND ISNULL(sp.IsDeleted, 0) = 0
				)
			AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
						INNER JOIN Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
					WHERE gs.OwnerId = @CreatedBy
						AND e.EmpId = @EmpId
						AND gs.ActionType = 5
						AND gs.RequestType IN (3, 4)
						AND ISNULL(gs.IsDeleted, 0) = 0
					)
					)
		BEGIN
			SET @ErrCode = 106
			SET @ErrMsg = 'You could not permision on this request.'

			RETURN;
		END
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
		SET @ErrCode = 109
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF @IsTopManager = 0
	BEGIN
		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor sp WITH (NOLOCK)
					WHERE sp.OwnerId = @VerifierId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 2
						AND sp.RequestType IN (3, 4)
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @VerifierId
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
					FROM dbo.EmployeeSupervisor sp WITH (NOLOCK)
					WHERE sp.OwnerId = @ApproverId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 3
						AND sp.RequestType IN (3, 4)
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @ApproverId
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

	INSERT INTO dbo.WorkingTimeRequest (
		[EmpId]
		,[RequestType]
		,[Remark]
		,[CurrentStatus]
		,[VerifierId]
		,[ApproverId]
		,[VerifiedStatus]
		,[ApprovedStatus]
		,[HRStatus]
		,[CreatedDate]
		,[CreatedBy]
		)
	VALUES (
		@EmpId
		,@RequestType
		,@Remark
		,CASE 
			WHEN @IsTopManager = 1
				AND @VerifierId IS NOT NULL THEN 1
			WHEN @IsTopManager = 1
				AND @ApproverId IS NOT NULL THEN 2
			WHEN @IsTopManager = 1
				AND @ApproverId IS NULL THEN 4
			ELSE 1
			END
		,@VerifierId
		,@ApproverId
		,CASE 
			WHEN @IsTopManager = 1
				AND @VerifierId IS NULL THEN 2
			ELSE 1
			END
		,CASE 
			WHEN @IsTopManager = 1
				AND @ApproverId IS NULL THEN 2
			ELSE 1
			END
		,1
		,@CreatedDate
		,@CreatedBy
		)

	SELECT @RequestId = cast(SCOPE_IDENTITY() AS BIGINT)

	IF (@RequestType IN (3, 4))
	BEGIN
		INSERT INTO dbo.LateInEarlyOutRequest (
			[RequestId]
			,[EmpId]
			,[OnDate]
			,[LateInMins]
			,[EarlyOutMins]
			,[ReasonId]
			)
		VALUES (
			@RequestId
			,@EmpId
			,@OnDate
			,@LateInMins
			,@EarlyOutMins
			,@ReasonId
			)
	END

	IF (
			@ApproverId IS NULL
			AND @VerifierId IS NULL
			AND @IsTopManager = 1
			)
	BEGIN
		EXECUTE dbo.LateInEarlyOutRequest_Approve @RequestID = @RequestID
				,@ProcessedBy = @EmpID
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
	END

	EXECUTE dbo.LateInEarlyOutRequest_WriteLog @RequestId = @RequestId

	SET @ErrCode = 0
END