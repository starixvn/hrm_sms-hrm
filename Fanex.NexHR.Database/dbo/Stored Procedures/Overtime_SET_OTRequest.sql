
CREATE PROCEDURE [dbo].[Overtime_SET_OTRequest] @EmpId INT
	,@RequestType SMALLINT = 8
	,@Remark VARCHAR(8000) = ''
	,@ProcessedBy INT
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@HRId INT = NULL
	,@ListDetail NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-04
Task: insert Over time request form
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
	@ListDetail:
		<Requests>
		 <Request
			OnDate="20130101"
			ShiftID="1"
			TimeIn="07:46"
			TimeOut=17:00"
			TotalHours="1"
			NormalHours="1"
			OffHours="1"
			PublicHours="1"
			Remark="ABC"
		  ></Request>
		</Requests>

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsTopManager BIT
		,@IsLocal BIT
		,@CanOT BIT
		,@ProcessedDate DATETIME

	SELECT TOP 1 @IsTopManager = IsTopManager
		,@IsLocal = IsLocal
		,@CanOT = CanOT
		,@ProcessedDate = GETDATE()
	FROM dbo.EmployeeSummary WITH (NOLOCK)
	WHERE EmpId = @EmpId

	IF (@CanOT = 0)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'You don''t have permission on this function.'

		RETURN
	END

	IF (
			@ApproverId IS NULL
			OR @VerifierId IS NULL
			)
		AND @IsTopManager = 0
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF OBJECT_ID('tempdb..#Calendar') IS NOT NULL
		DROP TABLE #Calendar

	CREATE TABLE #Calendar (
		OnDate DATETIME
		,ShiftID INT
		,TimeIn NVARCHAR(100)
		,[TimeOut] NVARCHAR(100)
		,TotalHours DECIMAL(18, 2)
		,NormalHours DECIMAL(18, 2)
		,OffHours DECIMAL(18, 2)
		,PublicHours DECIMAL(18, 2)
		,Remark NVARCHAR(1000)
		,IsOffDay BIT
		,TotalShiftHours DECIMAL(18, 2)
		,TotalWorkHours DECIMAL(18, 2)
		)

	DECLARE @DocHandler INT

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@ListDetail

		INSERT INTO #Calendar (
			OnDate
			,ShiftID
			,TimeIn
			,[TimeOut]
			,TotalHours
			,NormalHours
			,OffHours
			,PublicHours
			,Remark
			)
		SELECT OnDate
			,ShiftID
			,TimeIn
			,[TimeOut]
			,TotalHours
			,NormalHours
			,OffHours
			,PublicHours
			,REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Remark, '&amp;', '&'), '&#39;', ''''), '&lt;', '<'), '&gt;', '>'), '&quot;', '"')
		FROM OPENXML(@DocHandler, '/Requests/Request', 1) WITH (OnDate DATETIME, ShiftID INT, TimeIn NVARCHAR(100), [TimeOut] NVARCHAR(100), TotalHours DECIMAL(18, 2), NormalHours DECIMAL(18, 2), OffHours DECIMAL(18, 2), PublicHours DECIMAL(18, 2), Remark NVARCHAR(1000)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 101
			,@ErrMsg = N'Create overtime request for an employee failed.'

		RETURN
	END CATCH;

	IF NOT EXISTS (
			SELECT 1
			FROM #Calendar
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF @IsTopManager = 0
	BEGIN
		IF (
				NOT EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeSupervisor] AS sp WITH (NOLOCK)
					WHERE sp.OwnerId = @VerifierId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 2
						AND sp.RequestType IN (8)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @VerifierId
						AND g.ActionType = 2
						AND g.RequestType = 8
						AND ISNULL(g.IsDeleted, 0) = 0
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
					FROM [dbo].[EmployeeSupervisor] AS sp WITH (NOLOCK)
					WHERE sp.OwnerId = @ApproverId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = 3
						AND sp.RequestType IN (8)
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @ApproverId
						AND g.ActionType = 3
						AND g.RequestType = 8
						AND ISNULL(g.IsDeleted, 0) = 0
					)
				)
		BEGIN
			SET @ErrCode = 109
			SET @ErrMsg = 'invalid parameter ApproverId.'

			RETURN;
		END
	END

	

	UPDATE cl
	SET cl.TotalShiftHours = ISNULL(s.TotalHours, 0)
		,cl.TotalWorkHours = ISNULL(at.TotalWorkHours, 0)
		,cl.IsOffDay = (CASE 
			WHEN ec.IsHoliday = 1 THEN 1
			WHEN ec.IsOffDay = 1
				AND ec.IsKeepInView = 0 THEN 1
			ELSE 0 END
			)
	FROM #Calendar AS cl
		INNER JOIN dbo.Shift AS s WITH (NOLOCK) ON cl.ShiftID = s.ShiftID
		INNER JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON ec.WorkDate = cl.OnDate
		AND ec.EmpId = @EmpId
		INNER JOIN dbo.EmployeeAttendance AS at WITH (NOLOCK) ON at.AttendanceDate = cl.OnDate
		AND ec.EmpId = @EmpId

	SELECT TOP 1 @ErrMsg = CONVERT(NVARCHAR(10), ONDATE, 111)
	FROM #Calendar cl WITH (NOLOCK)
	WHERE (
			(cl.TotalHours - (cl.TotalWorkHours - cl.TotalShiftHours) < NormalHours + OffHours + PublicHours)
			AND cl.IsOffDay = 0
			)
		OR (
			(cl.TotalHours < NormalHours + OffHours + PublicHours)
			AND cl.IsOffDay = 1
			)

	SET @ErrMsg = ''

	DECLARE @RequestId BIGINT
		,@Month DATETIME
		,@CurrentStatus INT

	SELECT TOP 1 @Month = MAX(ot.OnDate)
	FROM #Calendar ot

	SELECT @Month = (CASE 
			WHEN DAY(@Month) BETWEEN 10
					AND 31 THEN DATEADD(MONTH, 1, @Month)
			ELSE @Month END
			)

	SELECT @Month = CONVERT(VARCHAR(10), YEAR(@Month)) + '/' + CONVERT(VARCHAR(10), MONTH(@Month)) + '/01 00:00:00'

	SELECT TOP 1 @RequestId = OT.RequestId
		,@CurrentStatus = OT.[CurrentStatus]
	FROM dbo.[OTMonthlyRequest] OT WITH (NOLOCK)
	WHERE [Month] = @Month
		AND OT.EmpId = @EmpId

	IF @CurrentStatus NOT IN (1, 3, 5, 7)
	BEGIN
		SET @ErrCode = 104
		SET @ErrMsg = 'The Status of this Request is not pending.'

		RETURN;
	END

	IF(NOT EXISTS (SELECT 1 FROM #Calendar WHERE ISNULL(NormalHours, 0) <> 0 OR ISNULL(OffHours, 0) <> 0 OR ISNULL(PublicHours, 0) <> 0))
	BEGIN
		
		DELETE dbo.[OTDailyRequest] WITH (ROWLOCK, UPDLOCK)
		WHERE EmpId = @EmpId
			AND [RequestId] = @RequestId

		DELETE dbo.[OTMonthlyRequest] WITH (ROWLOCK, UPDLOCK)
		WHERE [RequestId] = @RequestId

	END
	ELSE
	BEGIN

	IF @RequestId IS NULL
	BEGIN
		INSERT INTO [dbo].[OTMonthlyRequest] (
			[Month]
			,[EmpId]
			,[NormalHours]
			,[OffHours]
			,[PublicHours]
			,[CurrentStatus]
			,[VerifierId]
			,[ApproverId]
			,[HRId]
			,[VerifiedStatus]
			,[ApprovedStatus]
			,[HRStatus]
			,CreatedBy
			,CreatedDate
			)
		SELECT @Month
			,@EmpId
			,SUM(ISNULL([NormalHours], 0))
			,SUM(ISNULL([OffHours], 0))
			,SUM(ISNULL([PublicHours], 0))
			,1 [CurrentStatus]
			,@VerifierId
			,@ApproverId
			,@HRId
			,(CASE 
				WHEN @IsTopManager = 1
					AND @ApproverId IS NULL THEN 2
				ELSE 1 END
				) AS [VerifiedStatus]
			,(CASE 
				WHEN @IsTopManager = 1
					AND @VerifierId IS NULL THEN 2
				ELSE 1 END
				) AS [ApprovedStatus]
			,1 AS [HRStatus]
			,@ProcessedBy
			,@ProcessedDate
		FROM #Calendar rd

		SELECT @RequestId = cast(SCOPE_IDENTITY() AS BIGINT)
	END
	ELSE
	BEGIN
		UPDATE OT
		SET OT.[CurrentStatus] = (CASE 
				WHEN @IsTopManager = 1
					AND @VerifierId IS NOT NULL THEN 1
				WHEN @IsTopManager = 1
					AND @ApproverId IS NOT NULL THEN 2
				WHEN @IsTopManager = 1
					AND @ApproverId IS NULL THEN 4
				ELSE 1 END
				)
			,OT.VerifierId = @VerifierId
			,OT.[VerifiedStatus] = (CASE 
				WHEN @IsTopManager = 1
					AND @VerifierId IS NULL THEN 2
				ELSE 1 END
				)
			,OT.ApproverId = @ApproverId
			,OT.[ApprovedStatus] = (CASE 
				WHEN @IsTopManager = 1
					AND @ApproverId IS NULL THEN 2
				ELSE 1 END
				)
			,OT.HRId = @HRId
			,OT.ModifiedBy = @ProcessedBy
			,OT.ModifiedDate = @ProcessedDate
			,OT.[NormalHours] = rd.[NormalHours]
			,OT.[OffHours] = rd.[OffHours]
			,OT.[PublicHours] = rd.[PublicHours]
		FROM [OTMonthlyRequest] AS OT WITH (ROWLOCK, UPDLOCK)
			,(
				SELECT SUM(ISNULL(rd.[NormalHours], 0)) [NormalHours]
					,SUM(ISNULL(rd.[OffHours], 0)) [OffHours]
					,SUM(ISNULL(rd.[PublicHours], 0)) [PublicHours]
				FROM #Calendar AS rd
				) AS rd
		WHERE RequestId = @RequestId

		DELETE dbo.[OTDailyRequest] WITH (ROWLOCK, UPDLOCK)
		WHERE EmpId = @EmpId
			AND [RequestId] = @RequestId
	END

	INSERT INTO [dbo].[OTDailyRequest] (
		[OvertimeDate]
		,[EmpId]
		,[RequestId]
		,[Schedule]
		,[TimeIn]
		,[TimeOut]
		,[TotalHours]
		,[NormalHours]
		,[OffHours]
		,[PublicHours]
		,[Remark]
		,[Status]
		,[CreatedBy]
		,[CreatedDate]
		,[IsUpdated]
		)
	SELECT OnDate AS [OvertimeDate]
		,@EmpId AS [EmpId]
		,@RequestId AS [RequestId]
		,ShiftID AS [Schedule]
		,CONVERT(NVARCHAR(100), OnDate, 111) + ' ' + [TimeIn] + ':00' AS [TimeIn]
		,CONVERT(NVARCHAR(100), OnDate, 111) + ' ' + [TimeOut] + ':00' AS [TimeOut]
		,[TotalHours]
		,[NormalHours]
		,[OffHours]
		,[PublicHours]
		,[Remark]
		,1 AS [Status]
		,@ProcessedBy AS [CreatedBy]
		,@ProcessedDate AS [CreatedDate]
		,0 AS [IsUpdated]
	FROM #Calendar AS rd
	WHERE OnDate IS NOT NULL
		AND [TimeIn] IS NOT NULL
		AND [TimeOut] IS NOT NULL
		AND (
			[NormalHours] <> 0
			OR [OffHours] <> 0
			OR [PublicHours] <> 0
			)
	END
	IF @@ERROR <> 0
	BEGIN
		DELETE dbo.[OTDailyRequest]
		WHERE RequestId = @RequestId

		DELETE dbo.[OTMonthlyRequest]
		WHERE RequestId = @RequestId

		SELECT @ErrCode = 101
			,@ErrMsg = N'Create overtime request for an employee failed.'
	END
	ELSE
		SELECT @ErrCode = 0
			,@ErrMsg = ''

	DROP TABLE #Calendar
END