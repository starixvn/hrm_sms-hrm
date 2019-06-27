
CREATE PROCEDURE [dbo].[CheckInOutRequest_Insert] @EmpId INT
	,@RequestType SMALLINT
	,@Remark NVARCHAR(2000) = NULL
	,@OnDate DATETIME
	,@RequestTime DATETIME
	,@ReasonId TINYINT
	,@CreatedBy INT = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@RefNo NVARCHAR(200) = ''
	,@RequestId BIGINT = 0 OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190624
Task: create a check in/out request
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsTopManager BIT

	IF (ABS(DATEDIFF(DD, @OnDate, @RequestTime)) > 1)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Attendant date or Request time invalid.'

		RETURN;
	END

	-- check if there is another request in the same date
	IF (
			EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
					INNER JOIN dbo.CheckInOutRequest AS mis WITH (NOLOCK) ON wk.RequestId = mis.RequestId
				WHERE wk.empid = @empid
					AND RequestType = @RequestType
					AND CurrentStatus IN (1, 2, 4, 6)
					AND datediff(d, mis.OnDate, @OnDate) = 0
				)
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'There is another request in the same date'

		RETURN;
	END

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
		SET @ErrCode = 101
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF @EmpId <> @CreatedBy
	BEGIN
		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor sp WITH (NOLOCK)
					WHERE sp.OwnerId = @EmpId
						AND sp.EmpId = @CreatedBy
						AND sp.ActionType = 5
						AND sp.RequestType IN (1, 2)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
						INNER JOIN Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
					WHERE gs.OwnerId = @CreatedBy
						AND e.EmpId = @EmpId
						AND gs.ActionType = 5
						AND gs.RequestType IN (1, 2)
						AND ISNULL(gs.IsDeleted, 0) = 0
					)
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
					SELECT 1
					FROM dbo.EmployeeSupervisor sp WITH (NOLOCK)
					WHERE sp.OwnerId = @VerifierId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 2
						AND sp.RequestType IN (1, 2)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @VerifierId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (1, 2)
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
						AND ISNULL(sp.IsDeleted, 0) = 0
						AND sp.RequestType IN (1, 2)
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @ApproverId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (1, 2)
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
		,GETDATE()
		,@CreatedBy
		)

	SELECT @RequestId = cast(SCOPE_IDENTITY() AS BIGINT)

	INSERT INTO [dbo].[CheckInOutRequest] (
		[RequestId]
		,[EmpId]
		,[OnDate]
		,[RequestTime]
		,[RefNo]
		,[ReasonId]
		)
	VALUES (
		@RequestId
		,@EmpId
		,@OnDate
		,@RequestTime
		,@RefNo
		,@ReasonId
		)

	IF (
			@ApproverId IS NULL
			AND @VerifierId IS NULL
			AND @IsTopManager = 1
			)
	BEGIN
			EXECUTE dbo.CheckInOutRequest_Approve @RequestID = @RequestID
				,@ProcessedBy = @EmpID
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
	END

	EXECUTE dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId

	SET @ErrCode = 0
END