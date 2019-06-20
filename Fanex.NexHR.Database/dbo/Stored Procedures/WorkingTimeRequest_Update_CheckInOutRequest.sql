/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Update_CheckInOutRequest    Script Date: 7/20/2016 10:19:25 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Update_CheckInOutRequest    Script Date: 7/19/2016 5:22:24 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_Update_CheckInOutRequest] @RequestId BIGINT
	,@EmpId INT
	,@RequestType SMALLINT
	,@Remark NVARCHAR(2000) = NULL
	,@OnDate DATETIME = NULL
	,@CheckTime DATETIME = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@ModifiedBy INT = NULL
	,@RefNo NVARCHAR(200) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-04
Task: update a request form
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@ModifiedDate DATETIME

	SELECT @FromDate = @OnDate
		,@ToDate = @OnDate
		,@ModifiedDate = GETDATE()

	IF @CheckTime IS NULL
		SET @CheckTime = @OnDate

	IF EXISTS (
			SELECT TOP 1 1
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
			WHERE r.RequestId = @RequestId
				AND r.CurrentStatus <> 1
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Process awaiting approval. You could not edit this request.'

		RETURN;
	END

	IF @RequestType NOT IN (1, 2)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Late in request has the invalid parameter'

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
					AND r.RequestId <> @RequestId
					AND datediff(d, l.[OnDate], @FromDate) = 0
				)
			)
	BEGIN
		SET @ErrCode = 102
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
					AND r.RequestId <> @RequestId
				)
			)
	BEGIN
		SET @ErrCode = 110
		SET @ErrMsg = 'There is another request in the same date'

		RETURN;
	END

	DECLARE @IsTopManager BIT
		,@IsDefaultSchedule BIT
		,@IsLocal BIT

	SELECT @IsTopManager = IsTopManager
		,@IsDefaultSchedule = IsDefaultSchedule
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
		SET @ErrCode = 103
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF @EmpId <> @ModifiedBy
	BEGIN
		IF (
				NOT EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
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

	/*
DECLARE @SHIFTID INT
SELECT @SHIFTID = SHIFTID FROM dbo.EmployeeCalendar ec WITH(NOLOCK) WHERE WorkDate = CONVERT(NVARCHAR(100),@FromDate,111) AND [EmpId] = @EmpId

IF @SHIFTID IS NULL AND @IsDefaultSchedule = 1
BEGIN
	SELECT @SHIFTID =CASE DATEPART(dw,@FromDate) 
		WHEN 1 THEN ec.SunShift 
		WHEN 2 THEN ec.MonShift 
		WHEN 3 THEN ec.TueShift 
		WHEN 4 THEN ec.WedShift 
		WHEN 5 THEN ec.ThuShift 
		WHEN 6 THEN ec.FriShift 
		WHEN 1 THEN ec.SatShift 
		END
	 FROM dbo.EmployeeDefaultShift ec WITH(NOLOCK) WHERE ec.EmpID = @EmpId
END

DECLARE @BeginTime NVARCHAR(100),@EndTime NVARCHAR(100)
IF @SHIFTID IS NULL 
BEGIN
	SELECT @BeginTime = BeginCheckInTime , @EndTime=BeginCheckOutTime  FROM dbo.Shift WITH(NOLOCK) WHERE ShiftID = @SHIFTID 
END
*/
	UPDATE WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
	SET Remark = @Remark
		,[CurrentStatus] = CASE 
			WHEN @IsTopManager = 1
				AND @VerifierId IS NOT NULL THEN 1
			WHEN @IsTopManager = 1
				AND @ApproverId IS NOT NULL THEN 2
			WHEN @IsTopManager = 1
				AND @ApproverId IS NULL THEN 4
			ELSE 1
			END
		,VerifierId = @VerifierId
		,[VerifiedStatus] = CASE 
			WHEN @IsTopManager = 1
				AND @VerifierId IS NULL THEN 2
			ELSE 1
			END
		,-- pending [VerifiedStatus] as default
		ApproverId = @ApproverId
		,[ApprovedStatus] = CASE 
			WHEN @IsTopManager = 1
				AND @ApproverId IS NULL THEN 2
			ELSE 1
			END
		,-- pending [ApprovedStatus] as default
		ModifiedBy = @ModifiedBy
		,ModifiedDate = @ModifiedDate
	WHERE RequestId = @RequestId

	UPDATE [dbo].[InoutMissRequest] WITH (ROWLOCK, UPDLOCK)
	SET [EmpId] = @EmpId
		,[OnDate] = CONVERT(NVARCHAR(100), @FromDate, 111)
		,[CheckTime] = @CheckTime
		,[RefNo] = @RefNo
		,[ReworkCount] = [ReworkCount] + 1
	WHERE RequestId = @RequestId

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
				,@ProcessedDate = @ModifiedDate
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 2
			EXEC [dbo].[WorkingTimeRequest_ApproveRequest_2] @RequestID = @RequestID
				,@EmpID = @EmpID
				,@ProcessedBy = @EmpID
				,@ProcessedDate = @ModifiedDate
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
	END

	SELECT @ErrCode = 0
		,@ErrMsg = ''

	EXEC [WorkingTimeRequest_WriteHistory] @RequestId
END
