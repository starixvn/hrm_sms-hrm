/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Insert_CheckInOutRequest    Script Date: 7/20/2016 10:19:22 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Insert_CheckInOutRequest    Script Date: 7/19/2016 5:22:22 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_Insert_CheckInOutRequest] @EmpId INT
	,@RequestType SMALLINT
	,@Remark NVARCHAR(2000) = NULL
	,@OnDate DATETIME = NULL
	,@CheckTime DATETIME = NULL
	,@CreatedBy INT = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@RefNo NVARCHAR(200) = ''
	,@RequestId BIGINT = 0 OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-04
Task: insert Check In Out  request form
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@CreatedDate DATETIME
		
	SELECT @FromDate = @OnDate
		,@ToDate = @OnDate
		,@CreatedDate = GETDATE()

	IF @CheckTime IS NULL
		SET @CheckTime = @OnDate

	IF @RequestType NOT IN (1, 2)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Late in request has the invalid parameter'

		RETURN;
	END

	IF (ABS(DATEDIFF(DD, @OnDate, @CheckTime)) > 1)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Attendant date or Request time invalid.'

		RETURN;
	END

	IF (
			@RequestType IN (1, 2)
			AND @FromDate IS NULL
			)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'Missing check in/out request has the invalid parameter'

		RETURN;
	END

	-- check if there is another request in the same date
	IF (
			@RequestType IN (1, 2)
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
					INNER JOIN dbo.[InoutMissRequest] l WITH (NOLOCK) ON r.RequestId = l.RequestId
				WHERE r.empid = @empid
					AND RequestType = @RequestType
					AND CurrentStatus IN (1, 2, 3, 4, 6)
					AND datediff(d, l.[OnDate], @FromDate) = 0
				)
			)
	BEGIN
		SET @ErrCode = 106
		SET @ErrMsg = 'There is another request in the same date'

		RETURN;
	END

	-- check if there is another request in the same date
	IF (
			@RequestType IN (1, 2)
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
					INNER JOIN dbo.[InoutMissRequest] l WITH (NOLOCK) ON r.RequestId = l.RequestId
				WHERE r.empid = @empid
					AND RequestType = @RequestType
					AND CurrentStatus IN (1, 2, 4, 6)
					AND l.CheckTime = @CheckTime
				)
			)
	BEGIN
		SET @ErrCode = 110
		SET @ErrMsg = 'There is another request in the same date'

		RETURN;
	END

	DECLARE @IsTopManager BIT
		,@IsLocal BIT

	SELECT @IsTopManager = IsTopManager
		,@IsLocal = IsLocal
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
		SET @ErrCode = 107
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF @EmpId <> @CreatedBy
	BEGIN
		IF (
				NOT EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
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
					SELECT TOP 1 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
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
					SELECT TOP 1 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
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
		,-- pending [CurrentStatus] as default
		@VerifierId
		,@ApproverId
		,CASE 
			WHEN @IsTopManager = 1
				AND @VerifierId IS NULL THEN 2
			ELSE 1
			END
		,-- pending [VerifiedStatus] as default
		CASE 
			WHEN @IsTopManager = 1
				AND @ApproverId IS NULL THEN 2
			ELSE 1
			END
		,-- pending [ApprovedStatus] as default
		1
		,-- pending [HRStatus] as default
		@CreatedDate
		,@CreatedBy
		)

	SELECT @RequestId = cast(SCOPE_IDENTITY() AS BIGINT)

	IF (@RequestType IN (1, 2))
	BEGIN
		INSERT INTO [dbo].[InoutMissRequest] (
			[RequestId]
			,[EmpId]
			,[OnDate]
			,[CheckTime]
			,[RefNo]
			)
		VALUES (
			@RequestId
			,@EmpId
			,CONVERT(NVARCHAR(100), @FromDate, 111)
			,@CheckTime
			,@RefNo
			)
	END

	IF (
			@ApproverId IS NULL
			AND @VerifierId IS NULL
			AND @IsTopManager = 1
			)
	BEGIN
		IF @RequestType = 1
			EXEC [dbo].[WorkingTimeRequest_ApproveRequest_1] @RequestID = @RequestID
				,@EmpID = @EmpID
				,@ProcessedBy = @EmpID
				,@ProcessedDate = @CreatedDate
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 2
			EXEC [dbo].[WorkingTimeRequest_ApproveRequest_2] @RequestID = @RequestID
				,@EmpID = @EmpID
				,@ProcessedBy = @EmpID
				,@ProcessedDate = @CreatedDate
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
	END

	EXEC [WorkingTimeRequest_WriteHistory] @RequestId

	SET @ErrCode = 0
END
