
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[WorkingTimeRequest_Insert_ChangeShift] @EmpId INT
	,@RequestType SMALLINT = 5
	,@Remark NVARCHAR(2000)
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@CreatedBy INT
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@ListDetail NVARCHAR(MAX)
	,@RequestId BIGINT = 0 OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-04
Task: insert Change Shift request form
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	@ListDetail: '<ROOT><Request OnDate={0} FromShift={1} ToShift={2} Remark={3}></Request></ROOT>'

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CreatedDate DATETIME

	SET @CreatedDate = GETDATE()

	IF (@RequestType NOT IN (5))
	BEGIN
		SET @ErrCode = 104
		SET @ErrMsg = 'Change-shift request has the invalid parameter'

		RETURN;
	END

	IF (Datediff(dd, @FromDate, @ToDate) > 14)
	BEGIN
		SET @ErrCode = 105
		SET @ErrMsg = 'Change-shift request has the invalid parameter'

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
		SET @ErrCode = 106
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	-- check if there is another request in the same date
	IF (
			@RequestType IN (5)
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
					INNER JOIN dbo.ChangeShiftRequest l WITH (NOLOCK) ON r.RequestId = l.RequestId
				WHERE r.empid = @empid
					AND RequestType = @RequestType
					AND CurrentStatus IN (1, 2, 3, 4, 6)
					AND datediff(d, l.fromdate, @FromDate) = 0
				)
			)
	BEGIN
		SET @ErrCode = 107
		SET @ErrMsg = 'There is another request in the same date'

		RETURN;
	END

	CREATE TABLE #ChangeShiftDetail (
		OnDate DATETIME
		,FromShift INT
		,ToShift INT
		,Remark NVARCHAR(2000)
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@ListDetail

		INSERT INTO #ChangeShiftDetail (
			OnDate
			,FromShift
			,ToShift
			,Remark
			)
		SELECT OnDate
			,FromShift
			,ToShift
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Remark, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
		FROM OPENXML(@DocHandler, '/ROOT/Request', 1) WITH (OnDate DATETIME, FromShift INT, ToShift INT, Remark NVARCHAR(2000)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Create change shift request for an employee failed.'

		RETURN
	END CATCH

	IF @EmpId <> @CreatedBy
	BEGIN
		IF (
				NOT EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
					WHERE sp.OwnerId = @EmpId
						AND sp.EmpId = @CreatedBy
						AND sp.ActionType = 5
						AND sp.RequestType IN (5)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
						INNER JOIN Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
					WHERE gs.OwnerId = @CreatedBy
						AND e.EmpId = @EmpId
						AND gs.ActionType = 5
						AND gs.RequestType IN (5)
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
						AND sp.RequestType IN (5)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @VerifierId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (5)
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
						AND sp.RequestType IN (5)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @ApproverId
						AND ISNULL(g.IsDeleted, 0) = 0
						AND g.RequestType IN (5)
						AND g.ActionType = 3
					)
				)
		BEGIN
			SET @ErrCode = 109
			SET @ErrMsg = 'invalid parameter ApproverId.'

			RETURN;
		END
	END

	-- cut off time
	IF (@RequestType IN (5))
	BEGIN
		SET @fromdate = CONVERT(VARCHAR(10), @fromdate, 111)

		IF (@todate IS NOT NULL)
			SET @todate = CONVERT(VARCHAR(10), @todate, 111)
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

	INSERT INTO [dbo].[ChangeShiftRequest] (
		[RequestId]
		,[EmpId]
		,[FromDate]
		,[ToDate]
		)
	VALUES (
		@RequestId
		,@EmpId
		,@FromDate
		,@ToDate
		)

	INSERT INTO [dbo].[ChangeShiftRequestDetail] (
		[EmpId]
		,[OnDate]
		,[RequestId]
		,[FromShift]
		,[ToShift]
		,[Remark]
		)
	SELECT @EmpId
		,rd.OnDate
		,@RequestId RequestId
		,rd.FromShift
		,rd.ToShift
		,rd.Remark
	FROM #ChangeShiftDetail rd

	IF (
			@ApproverId IS NULL
			AND @VerifierId IS NULL
			AND @IsTopManager = 1
			)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest_5] @RequestID = @RequestID
			,@EmpID = @EmpID
			,@RoleID = 3
			,@ProcessedBy = @EmpID
			,@ProcessedDate = @CreatedDate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END

	EXEC [WorkingTimeRequest_WriteHistory] @RequestId

	IF @@ERROR <> 0
	BEGIN
		DELETE dbo.WorkingTimeRequest
		WHERE RequestId = @RequestId

		DELETE dbo.[ChangeShiftRequest]
		WHERE RequestId = @RequestId

		DELETE dbo.[ChangeShiftRequestDetail]
		WHERE RequestId = @RequestId

		SELECT @ErrCode = 101
			,@ErrMsg = N'Create change shift request for an employee failed.'
	END
	ELSE
		SELECT @ErrCode = 0
			,@ErrMsg = ''

	SET @ErrCode = 0

	DROP TABLE #ChangeShiftDetail
END
