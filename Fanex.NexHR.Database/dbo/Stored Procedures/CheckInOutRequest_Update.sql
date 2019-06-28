
CREATE PROCEDURE [dbo].[CheckInOutRequest_Update] @RequestId BIGINT
	,@EmpId INT
	,@RequestType SMALLINT
	,@Remark NVARCHAR(2000) = NULL
	,@OnDate DATETIME = NULL
	,@RequestTime DATETIME = NULL
	,@ReasonId TINYINT
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@ModifiedBy INT = NULL
	,@RefNo NVARCHAR(200) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190624
Task: Update a check in/out request
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @IsTopManager BIT
		,@ModifiedDate DATETIME = GETDATE()

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

	-- check if there is another request in the same date
	IF (
			@RequestType IN (1, 2)
			AND EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
					INNER JOIN dbo.CheckInOutRequest AS mis WITH (NOLOCK) ON wk.RequestId = mis.RequestId
				WHERE wk.empid = @empid
					AND RequestType = @RequestType
					AND CurrentStatus IN (1, 2, 4, 6)
					AND wk.RequestId <> @RequestId
					AND datediff(d, mis.OnDate, @OnDate) = 0
				)
			)
	BEGIN
		SET @ErrCode = 101
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
		SET @ErrCode = 103
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF @EmpId <> @ModifiedBy
	BEGIN
		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor sp WITH (NOLOCK)
					WHERE sp.OwnerId = @EmpId
						AND sp.EmpId = @ModifiedBy
						AND sp.ActionType = 5
						AND sp.RequestType IN (1, 2)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
						INNER JOIN Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
					WHERE gs.OwnerId = @ModifiedBy
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
						AND sp.RequestType IN (1, 2)
						AND ISNULL(sp.IsDeleted, 0) = 0
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

	UPDATE dbo.CheckInOutRequest WITH (ROWLOCK, UPDLOCK)
	SET EmpId = @EmpId
		,OnDate = CONVERT(NVARCHAR(100), @OnDate, 111)
		,RequestTime = @RequestTime
		,RefNo = @RefNo
		,ReasonId = @ReasonId
	WHERE RequestId = @RequestId

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