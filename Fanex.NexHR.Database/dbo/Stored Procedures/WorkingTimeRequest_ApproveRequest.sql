
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@IgnoredDetails NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-06
Task: approve request form (main SP)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
    @IgnoredDetails: 
        <ROOT>
            <Discipline WorkDate='' EmployeeId='' DisciplineTypeId='' IsIgnored='' IsConfirmed=''></Discipline>
        </ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@Status TINYINT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDay INT
		,@VerifierId INT
		,@ApproverId INT
		,@HRId INT
		,@VerifiedStatus INT
		,@ApprovedStatus INT
		,@HRStatus INT
		,@ProcessedDate DATETIME

	BEGIN TRY
		EXEC dbo.WorkingTimeRequest_GetRangeDateByRequestID @RequestId = @RequestId
			,@FromDate = @FromDate OUTPUT
			,@ToDate = @ToDate OUTPUT
			,@TotalDay = @TotalDay OUTPUT
			,@RequestType = @RequestType OUTPUT

		-- get employee and requestype of the request
		SELECT TOP 1 @EmpId = EmpId
			,@Status = [CurrentStatus]
			,@VerifierId = VerifierId
			,@VerifiedStatus = VerifiedStatus
			,@ApproverId = ApproverId
			,@ApprovedStatus = ApprovedStatus
			,@HRId = HRId
			,@HRStatus = HRStatus
		FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		WHERE RequestID = @RequestId

		SELECT @FromDate = convert(VARCHAR(10), @FromDate, 111)
			,@ToDate = convert(VARCHAR(10), @ToDate, 111)

		IF (
				@RoleId = 3
				AND @Status = 4
				)
		BEGIN
			RETURN
		END

		IF (
				@RoleId = 6
				AND ((@Status NOT IN (4)))
				)
		BEGIN
			SET @ErrCode = 103

			IF @Status NOT IN (4)
				SET @ErrMsg = 'This request have been approved or rejected.'
			ELSE IF @HRId <> @ProcessedBy
				SET @ErrMsg = 'You don''t have permission on this request.'

			RETURN
		END

		IF (@RequestType IN (1, 2))
		BEGIN
			IF EXISTS (
					SELECT TOP 1 1
					FROM WorkingTimeRequest r WITH (NOLOCK)
						INNER JOIN dbo.LeaveRequest lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
					WHERE r.empid = @EmpId
						AND r.RequestType = @RequestType
						AND r.CurrentStatus = 6
						AND lr.FromDate = @FromDate
					)
			BEGIN
				SET @ErrCode = 104
				SET @ErrMsg = 'Another approved request in the same date has existed'

				RETURN
			END
		END
		ELSE IF (@RequestType IN (3, 4))
		BEGIN
			IF EXISTS (
					SELECT TOP 1 1
					FROM WorkingTimeRequest r WITH (NOLOCK)
						INNER JOIN dbo.InOutRequest ior WITH (NOLOCK) ON r.RequestId = ior.RequestId
					WHERE r.empid = @EmpId
						AND r.RequestType = @RequestType
						AND r.CurrentStatus = 6
						AND ior.FromDate = @FromDate
					)
			BEGIN
				SET @ErrCode = 105
				SET @ErrMsg = 'Another approved request in the same date has existed'

				RETURN
			END
		END
		ELSE IF (@RequestType IN (5))
		BEGIN
			IF EXISTS (
					SELECT TOP 1 1
					FROM WorkingTimeRequest r WITH (NOLOCK)
						INNER JOIN dbo.ChangeShiftRequest cr WITH (NOLOCK) ON r.RequestId = cr.RequestId
					WHERE r.empid = @EmpId
						AND r.RequestType = @RequestType
						AND r.CurrentStatus = 6
						AND cr.FromDate = @FromDate
					)
			BEGIN
				SET @ErrCode = 106
				SET @ErrMsg = 'Another approved request in the same date has existed'

				RETURN
			END
		END

		-- if not exist calendar
		IF (
				@RequestType IN (1, 2, 3, 4, 5)
				AND NOT EXISTS (
					SELECT TOP 1 1
					FROM dbo.EmployeeCalendar WITH (NOLOCK)
					WHERE empid = @empid
						AND workdate = @fromdate
					)
				)
		BEGIN
			EXEC [dbo].[EmployeeCalendar_Insert] @empid = @empid
				,@fromdate = @FromDate
				,@todate = @fromdate
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

			IF @ErrCode <> 0
			BEGIN
				ROLLBACK TRANSACTION;

				RETURN
			END

			SET @ErrCode = 107
			SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'

			RETURN
		END
		ELSE IF (@RequestType = 6)
		BEGIN
			DECLARE @maxDate DATETIME
				,@minDate DATETIME
				,@result BIT

			SELECT @maxDate = max(workdate)
				,@minDate = min(workdate)
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE empid = @empid
				AND workdate BETWEEN @fromdate
					AND @todate

			IF (
					@minDate IS NULL
					OR @maxDate IS NULL
					OR @fromdate <> @minDate
					OR @todate <> @maxDate
					)
			BEGIN
				EXEC [dbo].[EmployeeCalendar_Insert] @empid = @empid
					,@fromdate = @FromDate
					,@todate = @todate
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT

				SET @ErrCode = 108
				SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'

				RETURN
			END

			DECLARE @TotalDays_tmp NUMERIC(5, 1)
				,@TotalDays_tmpbal NUMERIC(5, 1)
				,@isRquestDetail BIT = 1

			IF EXISTS (
					SELECT TOP 1 1
					FROM dbo.LeaveRequestDetail WITH (NOLOCK)
					WHERE RequestID = @RequestId
					)
			BEGIN
				SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay
							WHEN 1 THEN 0.5
							ELSE 1
							END)
				FROM dbo.EmployeeCalendar cl WITH (NOLOCK)
					INNER JOIN dbo.LeaveRequestDetail lrd WITH (NOLOCK) ON lrd.OnDate = cl.WorkDate
					AND lrd.EmpId = cl.EmpId
					INNER JOIN dbo.LeaveType l WITH (NOLOCK) ON lrd.LeaveType = l.LeaveTypeId
				WHERE lrd.RequestID = @RequestId
					AND cl.IsHoliday = 0
					AND cl.IsOffDay = 0
					AND lrd.WorkType = 2
					AND l.IsALDeduction = 1
			END
			ELSE
			BEGIN
				SET @isRquestDetail = 0

				SELECT @TotalDays_tmp = SUM(1)
				FROM dbo.EmployeeCalendar cl WITH (NOLOCK)
					INNER JOIN dbo.LeaveRequest lr WITH (NOLOCK) ON (
						cl.WorkDate BETWEEN CONVERT(SMALLDATETIME, convert(NVARCHAR(20), lr.FromDate, 111))
							AND CONVERT(SMALLDATETIME, convert(NVARCHAR(20), lr.ToDate, 111))
						)
					AND lr.EmpId = cl.EmpId
					INNER JOIN dbo.LeaveType l WITH (NOLOCK) ON lr.LeaveType = l.LeaveTypeId
				WHERE lr.RequestId = @RequestId
					AND cl.IsHoliday = 0
					AND cl.IsOffDay = 0
					AND l.IsALDeduction = 1
			END

			SET @TotalDays_tmp = ISNULL(@TotalDays_tmp, 0)

			SELECT @TotalDays_tmpbal = SUM(tmp.TotalDays)
			FROM dbo.tmpLeaveBalance tmp WITH (NOLOCK)
			WHERE tmp.RequestID = @RequestId

			IF (
					@TotalDays_tmp <> @TotalDays_tmpbal
					AND @isRquestDetail = 1
					)
				OR (
					@isRquestDetail = 0
					AND @TotalDays_tmp <> @TotalDays_tmpbal
					AND @TotalDays_tmpbal <> 0.5
					)
			BEGIN
				SET @ErrCode = 108
				SET @ErrMsg = 'Could not approved this request because this employee''s calendar have been changed.'

				RETURN
			END
		END

		--ignore
		EXEC dbo.WorkingTimeRequest_ApproveRequest_IgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ProcessedBy
			,@IgnoredDetails = @IgnoredDetails
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF @ErrCode = 0
		BEGIN
			SET @ProcessedDate = GETDATE()

			-- call appropriate SP for each request form
			IF (@RequestType = 1) -- check in
			BEGIN
				EXEC dbo.WorkingTimeRequest_ApproveRequest_1 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@RoleId = @RoleId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@Comment = @Comment
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 2) -- check out
			BEGIN
				EXEC dbo.WorkingTimeRequest_ApproveRequest_2 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@RoleId = @RoleId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@Comment = @Comment
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 3) -- late in
			BEGIN
				EXEC dbo.WorkingTimeRequest_ApproveRequest_3 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@RoleId = @RoleId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@Comment = @Comment
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 4) -- early out
			BEGIN
				EXEC dbo.WorkingTimeRequest_ApproveRequest_4 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@RoleId = @RoleId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@Comment = @Comment
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 5) -- change shift
			BEGIN
				EXEC dbo.WorkingTimeRequest_ApproveRequest_5 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@RoleId = @RoleId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@Comment = @Comment
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 6) -- leave
			BEGIN
				EXEC dbo.WorkingTimeRequest_ApproveRequest_6 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@RoleId = @RoleId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@Comment = @Comment
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 7) -- keep in view
			BEGIN
				EXEC dbo.WorkingTimeRequest_ApproveRequest_7 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@RoleId = @RoleId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@Comment = @Comment
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
		END
		ELSE
		BEGIN
			RETURN
		END

		-- write log
		IF @ErrCode = 0
		BEGIN
			DECLARE @LogSPData VARCHAR(4000)
				,@now DATETIME
				,@Description VARCHAR(200)

			SET @now = getdate()

			--Log general info
			SELECT @LogSPData = '<WorkingTimeRequest_ApproveRequest RequestID="' + coalesce(cast(@RequestId AS VARCHAR(50)), space(1)) + '" ' + 'ApprovedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" ' + 'ApprovedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" ' + 'RoleID="' + coalesce(cast(@RoleId AS VARCHAR(100)), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

			SELECT @Description = 'Approve Request:' + coalesce(cast(@RequestId AS VARCHAR(50)), space(1))

			EXEC dbo.Log_WriteLog @LogActionID = 1
				,@LogDate = @now
				,@Description = @Description
				,@TrackingValue = @LogSPData
				,@UserID = @ProcessedBy
				,@EmpID = @EmpID
		END
		ELSE
		BEGIN
			RETURN
		END
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove process failed'
	END CATCH;
END
