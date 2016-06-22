
CREATE PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForEmpId]
	@empId INT, 
	@fromTime DATETIME , 
	@toTime DATETIME ,
	@onlyUpdate BIT = 0
AS
BEGIN
	DECLARE @RangeToScanTime INT, @DateChangeRuleLateInEarlyOutRequest DATETIME
	SELECT @DateChangeRuleLateInEarlyOutRequest = '2015-10-19', @RangeToScanTime = 480 -- 480 munites

	DECLARE @TimeRecord TABLE (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT)
	INSERT INTO @TimeRecord	(Id, EmpID, RecordTime, InOutType)
	SELECT Id, EmpID, RecordTime, InOutType 
	FROM #TimeRecord
	WHERE RecordTime BETWEEN CONVERT(VARCHAR(10), DATEADD(HH, -2, @fromTime), 111) 
		AND CONVERT(VARCHAR(10), DATEADD(d, 2, @toTime), 111)
		AND EmpID = @empId
	--DELETE FROM #TimeRecord
	--WHERE RecordTime BETWEEN CONVERT(VARCHAR(10), @fromTime, 111) 
	--	AND CONVERT(VARCHAR(10), DATEADD(d, 2, @toTime), 111)
	--	AND EmpID = @empId

	DECLARE @i INT, @attendaceDate DATETIME
	DECLARE	@ID INT, 
		@EmpIdCur INT,
		@WorkDate DATETIME,
		@ShiftId INT,
		@IsHoliday BIT,
		@isOffDay BIT,
		@IsLeaveDay BIT,
		@LeaveTypeId INT,
		@LateIn INT,
		@EarlyOut INT,
		@RequestedCheckinTime DATETIME,
		@RequestedCheckoutTime DATETIME,
		@BeginCheckInTime DATETIME,
		@EndCheckInTime DATETIME,
		@BeginCheckOutTime DATETIME,
		@EndCheckOutTime DATETIME,
		@InOfMachine DATETIME,
		@OutOfMachine DATETIME,
		@ExpectedInTime DATETIME,
		@ExpectedOutTime DATETIME,
		@Absence BIT,
		@IsRightShift BIT,
		@ActualLateIn INT,
		@ActualEarlyOut INT,
		@TotalWorkHours DECIMAL(18,2),
		@BreakTimeMins INT,
		@LeaveDay DECIMAL(18,1),
		@Notes NVARCHAR(2000),
		@RequestLeaveTimeBegin DATETIME,
		@RequestLeaveTimeEnd DATETIME,
		@adjustLateIn INT,
		@adjustEarlyOut INT

	SET @i = 0
	SET @attendaceDate = CONVERT(VARCHAR(10), @fromTime, 111)
	WHILE (DATEDIFF(d, @attendaceDate, @toTime) >= 0)
	BEGIN 
		SELECT @Absence = 0,  @InOfMachine = null, @OutOfMachine = null, 
			@ID = null, @ExpectedOutTime  = null, @IsRightShift=null,
			@ExpectedInTime = null,	@ExpectedOutTime = null, 
			@ActualLateIn = 0, @ActualEarlyOut = 0,
			@RequestedCheckinTime = null, @RequestedCheckoutTime = null,
			@TotalWorkHours = null, @BreakTimeMins = 0,
			@ShiftId = null, @LeaveTypeId = null,
			@RequestLeaveTimeBegin = null, @RequestLeaveTimeEnd = null,
			@adjustLateIn = 0, @adjustEarlyOut = 0

		SELECT 
			@WorkDate = WorkDate,
			@ShiftId = ShiftId,
			@IsHoliday = IsHoliday,
			@isOffDay = IsOffDay,
			@IsLeaveDay = IsLeaveDay,
			@LeaveTypeId = LeaveTypeId,
			@LateIn = ISNULL(LateIn,0),
			@EarlyOut = ISNULL(EarlyOut,0),
			@RequestedCheckinTime = RequestedCheckinTime,
			@RequestedCheckoutTime = RequestedCheckoutTime,
			@BeginCheckInTime = BeginCheckInTime,
			@EndCheckInTime = EndCheckInTime,
			@BeginCheckOutTime = BeginCheckOutTime,
			@EndCheckOutTime = EndCheckOutTime,
			@BreakTimeMins  = BreakTimeMins,
			@LeaveDay = LeaveDay
		FROM #EmpCalendar
		WHERE EmpId = @empId AND WorkDate = @attendaceDate

		IF(DATEDIFF(D, @DateChangeRuleLateInEarlyOutRequest, @WorkDate) >= 0)
		BEGIN
			SELECT @LateIn = 0, @EarlyOut = 0
		END

		-- If Has Shift
		IF (@ShiftId IS NOT NULL AND ISNULL(@LeaveDay, 0) <> 1)
		BEGIN
			SET @ExpectedInTime = @BeginCheckInTime
			SET @ExpectedOutTime = @BeginCheckOutTime
			-- get time in
			SELECT @InOfMachine = MIN(RecordTime)
			FROM @TimeRecord 
			WHERE InOutType = 0
				AND RecordTime BETWEEN DATEADD(mi, -@RangeToScanTime, @ExpectedInTime) AND @ExpectedOutTime

			-- get time out
			SELECT @OutOfMachine = MAX(RecordTime)
			FROM @TimeRecord 
			WHERE InOutType = 1
				AND RecordTime BETWEEN @ExpectedInTime AND DATEADD(mi, @RangeToScanTime, @ExpectedOutTime)

			IF @InOfMachine > @OutOfMachine
			BEGIN
				SET @OutOfMachine = NULL
			END

			IF @RequestedCheckinTime IS NOT NULL
			BEGIN
				SET @InOfMachine = @RequestedCheckinTime
			END

			IF @RequestedCheckoutTime IS NOT NULL
			BEGIN
				SET @OutOfMachine = @RequestedCheckoutTime
			END

			IF (DATEADD(mi, @EarlyOut, @OutOfMachine) < @ExpectedOutTime)
			BEGIN
				SET @ActualEarlyOut = DATEDIFF (MINUTE, DATEADD(mi, @EarlyOut, @OutOfMachine), @ExpectedOutTime)	
			END
			IF (DATEADD(mi, -@LateIn, @InOfMachine) >  @ExpectedINTime) 
			BEGIN
				SET @ActualLateIn = DATEDIFF (MINUTE, @ExpectedINTime, DATEADD(mi,-@LateIn , @InOfMachine)) 
			END

			IF (@InOfMachine IS NOT NULL)
			BEGIN
				IF(@InOfMachine < @ExpectedInTime)
				BEGIN
					SET @TotalWorkHours =  (DATEDIFF(mi, @ExpectedInTime, @OutOfMachine) - ISNULL(@BreakTimeMins, 0))/ 60.0 
				END
				ELSE
				BEGIN
					SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine) - ISNULL(@BreakTimeMins, 0))/ 60.0 
				END
				IF(@IsLeaveDay = 1 and @LeaveDay = 0.5)
				BEGIN 
					SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
					FROM #LeaveRequestHalfDay
					WHERE EmpId = @empId AND OnDate = @attendaceDate

					IF(@RequestLeaveTimeBegin IS NOT NULL AND @RequestLeaveTimeEnd IS NOT NULL)
					BEGIN
						IF(DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) < 5 AND DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) > -5)
						BEGIN
							SET @adjustLateIn = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
							SET @ActualLateIn = @ActualLateIn - @adjustLateIn
							IF @ActualLateIn < 0
							BEGIN
								SET @ActualLateIn = 0
							END
						END
						ELSE IF (DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) < 5 AND DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) > -5)
						BEGIN
							SET @adjustEarlyOut = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
							SET @ActualEarlyOut = @ActualEarlyOut - @adjustEarlyOut
							IF @ActualEarlyOut < 0
							BEGIN
								SET @ActualEarlyOut = 0
							END
						END
					END
				END
				-- end xu ly nghi 1/2 ngay
			END
		END
		ELSE IF (ISNULL(@isOffDay, 0) = 0 AND ISNULL(@IsHoliday, 0) = 0 AND ISNULL(@LeaveDay, 0) <> 1)
		BEGIN -- No shift in attendance date 
			-- SET @ExpectedInTime = @attendaceDate
			-- SET @ExpectedOutTime = DATEADD(mi, 270, DATEADD(d, 1, @attendaceDate)) -- 4h30 of after day

			-- get time in
			SELECT @InOfMachine = MIN(RecordTime)
			FROM @TimeRecord 
			WHERE InOutType = 0
				AND RecordTime BETWEEN DATEADD(mi, 270, @attendaceDate) AND DATEADD(mi, 270, DATEADD(d, 1, @attendaceDate)) -- 4h30 of after day

			-- get time out
			SELECT @OutOfMachine = MAX(RecordTime)
			FROM @TimeRecord 
			WHERE InOutType = 1
				AND RecordTime BETWEEN DATEADD(mi, 540, @attendaceDate) AND DATEADD(mi, 720, DATEADD(d, 1, @attendaceDate)) -- 12h of after day

			IF @InOfMachine > @OutOfMachine
			BEGIN
				SET @OutOfMachine = NULL
			END

			IF @RequestedCheckinTime IS NOT NULL
			BEGIN
				SET @InOfMachine = @RequestedCheckinTime
			END

			IF @RequestedCheckoutTime IS NOT NULL
			BEGIN
				SET @OutOfMachine = @RequestedCheckoutTime
			END

			IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
			BEGIN
				SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine))/ 60.0 
			END
		END

		IF (@isOffDay = 1 OR @IsHoliday = 1 OR (@IsLeaveDay = 1 AND @LeaveDay = 1))
		BEGIN
			SET @Absence = 1
		END
		ELSE IF (@InOfMachine IS NULL and @OutOfMachine IS NULL)
		BEGIN
			SET @Absence = 1
		END

		--SELECT @ShiftId, @InOfMachine, @OutOfMachine, @IsLeaveDay, @isOffDay, @IsHoliday
		IF(DATEDIFF(DD, @attendaceDate, GETDATE()) >= 0)
		BEGIN
			IF(@ShiftId IS NOT NULL OR @InOfMachine IS NOT NULL OR @OutOfMachine IS NOT NULL OR @IsLeaveDay = 1 OR @isOffDay = 1 OR @IsHoliday = 1)
			BEGIN
				-- insert or update
				IF (EXISTS(SELECT 1 FROM dbo.EmployeeAttendance WITH (NOLOCK) 
					WHERE EmpId = @empId AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0))
				BEGIN
					--update
					UPDATE dbo.EmployeeAttendance
					SET 
						ShiftId = @ShiftId, 
						InFixed = @InOfMachine,
						OutFixed = @OutOfMachine,
						ExpectedInTime = @ExpectedInTime,
						ExpectedOutTime = @ExpectedOutTime,
						LateIn  = @ActualLateIn,
						EarlyOut = @ActualEarlyOut,
						LeaveTypeId = @LeaveTypeId,
						Absence = @Absence,
						TotalWorkHours = @TotalWorkHours,
						IsRightShift = @IsRightShift,
						LeaveDay = @LeaveDay,
						InsertDate = GETDATE()
					WHERE EmpId = @empId AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0
				END
				ELSE
				BEGIN
					--insert
					INSERT INTO dbo.EmployeeAttendance
					(
						EmpId,
						AttendanceDate,
						ShiftId,
						InFixed,
						OutFixed,
						ExpectedInTime,
						ExpectedOutTime,
						LateIn,
						EarlyOut,
						LeaveTypeId,
						Absence,
						TotalWorkHours,
						IsRightShift,
						LeaveDay,
						InsertDate
					)
					VALUES
					(
						@empId, 
						@attendaceDate,
						@ShiftId,
						@InOfMachine,
						@OutOfMachine,
						@ExpectedInTime,
						@ExpectedOutTime,
						@ActualLateIn,
						@ActualEarlyOut,
						@LeaveTypeId,
						@Absence,
						@TotalWorkHours,
						@IsRightShift,
						@LeaveDay,
						GETDATE()
					)			
				END
			END
			ELSE
			BEGIN
				DELETE FROM dbo.EmployeeAttendance WHERE EmpId = @empId and DATEDIFF(d, AttendanceDate, @attendaceDate) = 0
			END

			--EXECUTE [dbo].[EmployeeAttendanceViolation_Insert] @empId, @attendaceDate
		END
		SET @i = @i + 1
		SET @attendaceDate = DATEADD(d, @i, @fromTime)
	END
END