
CREATE PROCEDURE [dbo].[KeepInViewRequest_Insert] @EmpId INT
	,@Remark NVARCHAR(2000) = NULL
	,@OnDate DATETIME = NULL
	,@KeepInViewType TINYINT = NULL
	,@IsHalfDay BIT = NULL
	,@FromTime DATETIME = NULL
	,@ToTime DATETIME = NULL
	,@CreatedBy INT = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@RefNo NVARCHAR(200) = ''
	,@RequestId BIGINT = 0 OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190522
Task: insert a Keep inview request by day
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CreatedDate DATETIME
		,@RequestType SMALLINT = 7

	SELECT @CreatedDate = GETDATE()

	IF (
			@KeepInViewType = 1
			AND @IsHalfDay = 0
			)
	BEGIN
		SELECT @FromTime = NULL
			,@ToTime = NULL
	END

	IF (
			(
				@KeepInViewType = 2
				OR @IsHalfDay = 1
				)
			AND (
				@FromTime IS NULL
				OR @ToTime IS NULL
				)
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Keep In request has the invalid parameter'

		RETURN;
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
				INNER JOIN dbo.[KeepInViewRequest] l WITH (NOLOCK) ON r.RequestId = l.RequestId
			WHERE r.empid = @empid
				AND r.CurrentStatus IN (1, 2, 4, 6)
				AND DATEDIFF(DD, @OnDate, l.OnDate) = 0
			)
	BEGIN
		SET @ErrCode = 105
		SET @ErrMsg = 'There is another request in the same range'

		RETURN;
	END

	IF (@KeepInViewType = 1 AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE WorkDate = @OnDate
				AND EmpId = @EmpId
				AND (
					IsHoliday = 1
					OR IsOffDay = 1
					)
			)
			)
	BEGIN
		SET @ErrCode = 106
		SET @ErrMsg = CONVERT(NVARCHAR(100), @OnDate, 106) + '  not off date or holiday '

		RETURN
	END

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.OTDailyRequest OT WITH (NOLOCK)
				WHERE OT.OvertimeDate = @OnDate
					AND EmpId = @EmpId
					AND (
						NormalHours > 0
						OR OffHours > 0
						OR PublicHours > 0
						)
				)
			)
	BEGIN
		SET @ErrCode = 107
		SET @ErrMsg = 'You have registered overtime on  ' + CONVERT(NVARCHAR(100), @OnDate, 106) + '.'

		RETURN
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
		SET @ErrCode = 101
		SET @ErrMsg = 'Keep In View request has the invalid parameter'

		RETURN;
	END

	IF @EmpId <> @CreatedBy
	BEGIN
		IF (
				NOT EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
					WHERE sp.OwnerId = @EmpId
						AND sp.EmpId = @CreatedBy
						AND sp.ActionType = 5
						AND sp.RequestType IN (7)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
						INNER JOIN Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
					WHERE gs.OwnerId = @CreatedBy
						AND e.EmpId = @EmpId
						AND gs.ActionType = 5
						AND gs.RequestType IN (7)
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
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
					WHERE sp.OwnerId = @VerifierId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 2
						AND sp.RequestType IN (7)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @VerifierId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (7)
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
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
					WHERE sp.OwnerId = @ApproverId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 3
						AND sp.RequestType IN (7)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @ApproverId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (7)
						AND g.ActionType = 3
					)
				)
		BEGIN
			SET @ErrCode = 109
			SET @ErrMsg = 'invalid parameter ApproverId.'

			RETURN;
		END
	END

	IF (DATEDIFF(MI, @FromTime, @ToTime) < 60)
	BEGIN
		SET @ErrCode = 110
			SET @ErrMsg = 'Min of KIV is 60 minutes.'

			RETURN;
	END

	EXEC [dbo].WorkingTimeRequest_GetNewRefNoByRequestType @RefNo = @RefNo OUTPUT
		,@RequestType = 7

	BEGIN TRY
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

		INSERT INTO [dbo].[KeepInViewRequest] (
			[RequestId]
			,[EmpId]
			,[OnDate]
			,[KeepInViewType]
			,[IsHalfDay]
			,[FromTime]
			,[ToTime]
			,[RefNo]
			)
		VALUES (
			@RequestId
			,@EmpId
			,@OnDate
			,@KeepInViewType
			,@IsHalfDay
			,@FromTime
			,@ToTime
			,@RefNo
			)

		IF (
				@ApproverId IS NULL
				AND @VerifierId IS NULL
				AND @IsTopManager = 1
				)
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_ApproveRequest_7] @RequestID = @RequestID
				,@EmpID = @EmpID
				,@ProcessedBy = @EmpID
				,@ProcessedDate = @CreatedDate
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END

		EXEC dbo.KeepInViewRequest_WriteLog @RequestId
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 101
			,@ErrMsg = N'Create keep in request for an employee failed.'

		DELETE dbo.WorkingTimeRequest
		WHERE RequestId = @RequestId

		DELETE dbo.[KeepInViewRequest]
		WHERE RequestId = @RequestId

		RETURN
	END CATCH;

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END