
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Insert_LateInEarlyOutRequest    Script Date: 7/20/2016 10:19:22 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Insert_LateInEarlyOutRequest    Script Date: 7/19/2016 5:22:22 PM ******/

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[WorkingTimeRequest_Insert_LateInEarlyOutRequest] @EmpId INT
	,@RequestType SMALLINT
	,@Remark NVARCHAR(2000) = NULL
	,@OnDate DATETIME = NULL
	,@LateInMins INT = NULL
	,@EarlyOutMins INT = NULL
	,@CreatedBy INT = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@RequestId BIGINT = 0 OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-04
Task: insert Late In/Early Out request form
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

	IF @RequestType NOT IN (3, 4)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'Late in request has the invalid parameter'

		RETURN;
	END

	IF (
			@RequestType IN (3)
			AND (
				@FromDate IS NULL
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
				@FromDate IS NULL
				OR isnull(@EarlyOutMins, 0) <= 0
				OR isnull(@EarlyOutMins, 0) > 60
				)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Late out request has the invalid parameter'

		RETURN;
	END

	DECLARE @_FromDate NVARCHAR(100)
		,@_ToDate NVARCHAR(100)

	SELECT @_FromDate = CONVERT(VARCHAR(10), @fromDate, 111)
		,--+ ' 00:00:00',
		@_ToDate = CONVERT(VARCHAR(10), @toDate, 111) --+ ' 00:00:00'

	IF (
			@RequestType IN (3, 4)
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
					INNER JOIN dbo.InOutRequest l WITH (NOLOCK) ON r.RequestId = l.RequestId
				WHERE r.empid = @empid
					AND r.RequestType = @RequestType
					AND r.CurrentStatus IN (1, 2, 3, 4, 6)
					AND (
						CONVERT(VARCHAR(10), l.fromdate, 111) BETWEEN @_FromDate
							AND @_ToDate
						OR CONVERT(VARCHAR(10), l.todate, 111) BETWEEN @_FromDate
							AND @_ToDate
						OR @_FromDate BETWEEN CONVERT(VARCHAR(10), l.fromdate, 111)
							AND CONVERT(VARCHAR(10), l.todate, 111)
						OR @_ToDate BETWEEN CONVERT(VARCHAR(10), l.fromdate, 111)
							AND CONVERT(VARCHAR(10), l.todate, 111)
						)
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
				SELECT TOP 1 1
				FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
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
		SET @ErrCode = 109
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
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
					SELECT TOP 1 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
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

	IF (@RequestType IN (3, 4))
	BEGIN
		INSERT INTO [dbo].[InOutRequest] (
			[RequestId]
			,[EmpId]
			,[LateInMins]
			,[EarlyOutMins]
			,[FromDate]
			,[ToDate]
			)
		VALUES (
			@RequestId
			,@EmpId
			,@LateInMins
			,@EarlyOutMins
			,@FromDate
			,@ToDate
			)
	END

	IF (
			@ApproverId IS NULL
			AND @VerifierId IS NULL
			AND @IsTopManager = 1
			)
	BEGIN
		IF @RequestType = 3
			EXEC [dbo].[WorkingTimeRequest_ApproveRequest_3] @RequestID = @RequestID
				,@EmpID = @EmpID
				,@ProcessedBy = @EmpID
				,@ProcessedDate = @CreatedDate
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 4
			EXEC [dbo].[WorkingTimeRequest_ApproveRequest_4] @RequestID = @RequestID
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
