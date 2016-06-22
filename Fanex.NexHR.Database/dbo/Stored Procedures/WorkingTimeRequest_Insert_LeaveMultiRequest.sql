
CREATE PROCEDURE [dbo].[WorkingTimeRequest_Insert_LeaveMultiRequest]
	@EmpId int, 
	@RequestType smallint,	

	@Remark nvarchar(2000)=NULL,	
	@FromDate datetime=NULL, 
	@ToDate datetime=NULL, 
	@LeaveTypeID int=NULL, 
	@TotalDays numeric(5,1)=NULL,

	@CreatedBy int=NULL,

	@VerifierId  int=NULL,
    @ApproverId  int=NULL,

	@RefNo nvarchar(200)='',
	@ListDetail varchar(MAX),--OnDate1,ShiftID1,LeaveType1,IsHalfDay1,FromTime1,ToTime1@OnDate2,ShiftID2,LeaveType2,IsHalfDay2,FromTime2,ToTime2@..@OnDaten,ShiftIDn,LeaveTypen,IsHalfDayn,FromTimen,ToTimen
	
	@RequestId BIGINT = 0 OUTPUT,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT	
AS
/*
Created: Tom.Vo@2013-11-11
Task: insert a Leave mutilple request form
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	- @ListDetail: '<ROOT><LeaveRequestDetail OnDate='{20130101}' ShiftID='{1}' LeaveType={2} IsHalfDay={0} WorkType={0} FromTime={20130101 00:00:01} ToTime={20130101 00:00:01}></LeaveRequestDetail></ROOT>'
	- @WorkType :
			1. off
			2. leave
			3. work
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @TotalDays_tmp numeric(5,1),@isHalfDay BIT, @CreatedDate DATETIME


	SET @isHalfDay = CASE  WHEN @TotalDays = 0.5 THEN 1 ELSE 0 END

	SET @TotalDays_tmp = @TotalDays

	SET @CreatedDate = ISNULL(@CreatedDate,GETDATE())

	IF (@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND YEAR(@FromDate) <> YEAR(@ToDate))
	BEGIN
		SET @ErrCode =103
		SET @ErrMsg = 'Do not cross period.'
		RETURN;
	END

	IF (@RequestType IN (6) AND 
		(@FromDate IS NULL OR @ToDate IS NULL OR @LeaveTypeID IS NULL 
			OR ISNULL(@TotalDays,0) < 0 )
		)
	BEGIN
		SET @ErrCode =104
		SET @ErrMsg = 'Leave request has the invalid parameter'
		RETURN;
	END

	DECLARE @IsTopManager BIT,@IsLocal  BIT
	SELECT TOP 1 @IsTopManager = IsTopManager,@IsLocal=IsLocal from dbo.EmployeeSummary WITH(NOLOCK) WHERE EmpId = @EmpId

	IF ((@ApproverId IS NULL OR @VerifierId IS NULL) AND @IsTopManager = 0)
	BEGIN
		SET @ErrCode =105
		SET @ErrMsg = 'Invalid parameter'
		RETURN;
	END

	DECLARE	@_FromDate NVARCHAR(100), @_ToDate NVARCHAR(100)

	SELECT @_FromDate = CONVERT(VARCHAR(10),@fromDate,111) + ' 00:00:00',
			@_ToDate = CONVERT(VARCHAR(10),@toDate,111) + ' 00:00:00'

	CREATE TABLE #LeaveRequestDetail (OnDate DateTime,ShiftID	INT,LeaveType	INT,WorkType TINYINT,IsHalfDay BIT,FromTime DateTime,ToTime DateTime)
	DECLARE @DocHandler INT

	BEGIN TRY	
		EXEC sp_xml_preparedocument @DocHandler OUTPUT, @ListDetail

		IF DATEDIFF (DD,@_FromDate,@_ToDate)+1 <> (SELECT COUNT(1) FROM dbo.EmployeeCalendar WITH(NOLOCK)
							WHERE EmpID= @EmpID AND WorkDate BETWEEN @_FromDate AND @_ToDate )
			BEGIN
				EXEC [dbo].[EmployeeCalendar_Insert_ByLeaveRequest]
						@empid =@EmpID,
						@fromdate =@_FromDate, 
						@todate =@_ToDate
			END

		INSERT INTO #LeaveRequestDetail (OnDate,ShiftID,LeaveType,WorkType,IsHalfDay,FromTime,ToTime)
		SELECT Data.OnDate,ISNULL(Data.ShiftID,ec.ShiftId) ShiftID,Data.LeaveType,Data.WorkType,Data.IsHalfDay,Data.FromTime,Data.ToTime
		FROM OPENXML (@DocHandler, '/ROOT/LeaveRequestDetail',1) 
		WITH (	OnDate	DateTime,
				ShiftID	INT, 
				LeaveType	INT,
				IsHalfDay	BIT,
				WorkType	TINYINT,
				FromTime	DateTime,
				ToTime	DateTime) AS Data 
				LEFT JOIN dbo.EmployeeCalendar ec WITH(NOLOCK)
					ON ec.WorkDate = data.OnDate AND ec.EmpId = @EmpId 

		IF @@ROWCOUNT <> 0
			SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay WHEN 1 THEN 0.5 ELSE 1 END) 
			FROM dbo.EmployeeCalendar cl WITH(NOLOCK)
						INNER JOIN #LeaveRequestDetail lrd ON cl.WorkDate = lrd.OnDate
								WHERE EmpID= @EmpID AND WorkDate BETWEEN @_FromDate AND @_ToDate
								AND IsHoliday = 0 AND IsOffDay = 0 AND IsLeaveDay = 0
								AND lrd.WorkType =2
			IF @TotalDays_tmp IS NULL
				SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay WHEN 1 THEN 0.5 ELSE 1 END) 
					FROM  #LeaveRequestDetail lrd
					WHERE lrd.WorkType =2

		EXEC sp_xml_removedocument @DocHandler

	END TRY
	BEGIN CATCH
			print ERROR_MESSAGE ( ) 
			SELECT @ErrCode = 100, @ErrMsg = N'Create leave request for an employee failed.'
			Return
	END CATCH;

	IF EXISTS(SELECT TOP 1 1 FROM #LeaveRequestDetail 
				WHERE IsHalfDay=1
				GROUP BY OnDate
				Having Count(DISTINCT LeaveType)=1  AND COUNT(OnDate)>1
	)
	BEGIN
			SELECT @ErrCode = 101, @ErrMsg = N'Please check Leave Type on this request.'
			Return
	END

	IF NOT EXISTS(SELECT TOP 1 1 FROM #LeaveRequestDetail)
	BEGIN
		SELECT @TotalDays_tmp = SUM(1) FROM dbo.EmployeeCalendar cl WITH(NOLOCK)
					WHERE EmpID= @EmpID AND WorkDate BETWEEN @_FromDate AND @_ToDate
					AND IsHoliday = 0 AND IsOffDay = 0 
					--AND ShiftId IS NOT NULL 
					--AND IsLeaveDay = 0
		IF @isHalfDay = 1 AND ISNULL(@TotalDays_tmp,1) = 1 
			SET @TotalDays_tmp = 0.5
	END

	IF (ISNULL(@TotalDays_tmp,0) <=0 AND @LeaveTypeID <> 13)
		BEGIN
			SET @ErrCode =105
			SET @ErrMsg = 'Your leave days invalid.'
			RETURN;
		END

	IF(@LeaveTypeID <> 13 AND @TotalDays = 0.5 AND 
		EXISTS(SELECT 1 FROM dbo.EmployeeCalendar WITH(NOLOCK)
				WHERE EmpID= @EmpId AND WorkDate = @FromDate
				AND (IsHoliday = 1 OR IsOffDay = 1)))
	BEGIN
		SET @ErrCode =105
			SET @ErrMsg = 'Your can not request on off day or holiday.'
			RETURN;
	END

	IF @TotalDays_tmp IS NOT NULL OR @TotalDays_tmp >0
		SET @TotalDays = @TotalDays_tmp 

	IF @EmpId <> @CreatedBy
	BEGIN
		IF (NOT EXISTS (SELECT TOP 1 1
		FROM [dbo].[EmployeeSupervisor] sp WITH(NOLOCK)
		WHERE sp.OwnerId = @EmpId
			AND sp.EmpId = @CreatedBy
			AND sp.ActionType = 5
			AND sp.RequestType = 6
			AND ISNULL(sp.IsDeleted , 0) = 0
			)
		AND NOT EXISTS(SELECT 1 FROM dbo.GroupSupervisor AS g with (nolock)
			inner join dbo.Employee AS e with (nolock) on g.GroupId = e.GroupId
			where e.EmpId = @EmpId
			AND g.OwnerId = @CreatedBy
			and g.RequestType = 6 
			and g.ActionType = 5
			AND ISNULL(g.IsDeleted , 0) = 0)
			)
		BEGIN
			SET @ErrCode = 106
			SET @ErrMsg = 'You could not permision on this request.'
			RETURN;
		END
	END

	IF @IsTopManager = 0
	BEGIN
		IF (NOT EXISTS (SELECT TOP 1 1
		FROM [dbo].[EmployeeSupervisor] sp WITH(NOLOCK)
		WHERE sp.OwnerId = @VerifierId
			AND sp.EmpId = @EmpId
			AND sp.ActionType = 2
			AND sp.RequestType in (6)
			AND ISNULL(sp.IsDeleted , 0) = 0
			)
		AND NOT EXISTS(SELECT 1 FROM dbo.GroupSupervisor AS g with (nolock)
			inner join dbo.Employee AS e with (nolock) on g.GroupId = e.GroupId
			where e.EmpId = @EmpId
			AND g.OwnerId = @VerifierId
			and g.RequestType = 6 
			and g.ActionType = 2
			AND ISNULL(g.IsDeleted , 0) = 0)
		)
		BEGIN
			SET @ErrCode = 108
			SET @ErrMsg = 'invalid parameter verifierid.'
			RETURN;
		END
		IF (NOT EXISTS (SELECT TOP 1 1
		FROM [dbo].[EmployeeSupervisor] sp WITH(NOLOCK)
		WHERE sp.OwnerId = @ApproverId
			AND sp.EmpId = @EmpId
			AND sp.ActionType = 3
			AND sp.RequestType = 6
			AND ISNULL(sp.IsDeleted , 0) = 0
			)
		AND NOT EXISTS(SELECT 1 FROM dbo.GroupSupervisor AS g with (nolock)
			inner join dbo.Employee AS e with (nolock) on g.GroupId = e.GroupId
			where e.EmpId = @EmpId
			AND g.OwnerId = @ApproverId
			and g.RequestType = 6 
			and g.ActionType = 3
			AND ISNULL(g.IsDeleted , 0) = 0)
		)
		BEGIN
			SET @ErrCode = 109
			SET @ErrMsg = 'invalid parameter ApproverId.'
			RETURN;
		END
	END

	IF (@RequestType = 6) AND 
		(EXISTS(SELECT 1 FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						INNER JOIN dbo.LeaveRequest l WITH(NOLOCK) ON r.RequestId = l.RequestId
						LEFT JOIN dbo.EmployeeCalendar c WITH(NOLOCK) ON c.WorkDate BETWEEN l.fromdate AND l.ToDate AND c.empid = l.EmpId
				WHERE r.empid=@empid 
				AND r.RequestType=@RequestType
				AND CurrentStatus IN (1,2,4,6)
				AND (
						(@TotalDays >= 1 AND 
							(CONVERT(VARCHAR(10),fromdate,111) BETWEEN CONVERT(VARCHAR(10),@fromdate,111) AND CONVERT(VARCHAR(10),@todate,111)  
							OR CONVERT(VARCHAR(10),todate,111) BETWEEN CONVERT(VARCHAR(10),@fromdate,111) AND CONVERT(VARCHAR(10),@todate,111)
							OR CONVERT(VARCHAR(10),@fromdate,111) BETWEEN CONVERT(VARCHAR(10),fromdate,111) AND CONVERT(VARCHAR(10),todate,111)
							OR CONVERT(VARCHAR(10),@todate,111) BETWEEN CONVERT(VARCHAR(10),fromdate,111) AND CONVERT(VARCHAR(10),todate,111))
						)
						OR
						(
							@TotalDays < 1 AND TotalDays >= 1 AND
							(CONVERT(VARCHAR(10),fromdate,111) BETWEEN CONVERT(VARCHAR(10),@fromdate,111) AND CONVERT(VARCHAR(10),@todate,111)  
							OR CONVERT(VARCHAR(10),todate,111) BETWEEN CONVERT(VARCHAR(10),@fromdate,111) AND CONVERT(VARCHAR(10),@todate,111)
							OR CONVERT(VARCHAR(10),@fromdate,111) BETWEEN CONVERT(VARCHAR(10),fromdate,111) AND CONVERT(VARCHAR(10),todate,111)
							OR CONVERT(VARCHAR(10),@todate,111) BETWEEN CONVERT(VARCHAR(10),fromdate,111) AND CONVERT(VARCHAR(10),todate,111))
						)
					)
		) 
		OR
			EXISTS (SELECT 1 FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						INNER JOIN dbo.LeaveRequest l WITH (NOLOCK) ON r.RequestId = l.RequestId 
						INNER JOIN dbo.LeaveRequestDetail ldt WITH (NOLOCK) ON r.RequestId = ldt.RequestId
						INNER JOIN #LeaveRequestDetail ldtt WITH(NOLOCK) ON ldtt.OnDate = ldt.OnDate
				WHERE r.empid=@empid 
				AND r.RequestType=@RequestType
				AND CurrentStatus IN (1,2,4,6)
				AND @TotalDays = 0.5
				AND l.TotalDays = 0.5
				AND ((ldt.FromTime BETWEEN ldtt.FromTime AND ldtt.ToTime)
						OR (ldt.ToTime BETWEEN ldtt.FromTime AND ldtt.ToTime)
						OR (ldtt.FromTime BETWEEN ldt.FromTime AND ldt.ToTime)
						OR (ldtt.ToTime BETWEEN ldt.FromTime AND ldt.ToTime)
					)
				AND ldt.FromTime <> ldtt.ToTime
				AND ldt.ToTime <> ldtt.FromTime
				)
		)
	BEGIN
		SET @ErrCode = 107
		SET @ErrMsg = 'There is another request in the same range'
		RETURN;
	END

	DECLARE @AnnualLeave decimal(18,2),@AnnualLeavePending decimal(18,2),@AnnualLeaveBalance decimal(18,2),@KIVLeave decimal(18,2),@KIVLeaveBalance decimal(18,2)
	IF EXISTS(SELECT TOP 1 1 FROM #LeaveRequestDetail)
		SELECT @AnnualLeave = SUM(CASE cl.IsHalfDay WHEN 1 THEN 0.5 ELSE 1 END) 
		FROM #LeaveRequestDetail cl WITH(NOLOCK)
		INNER JOIN dbo.LeaveType l WITH(NOLOCK)  ON cl.LeaveType = l.LeaveTypeId
		WHERE  IsALDeduction = 1
	ELSE IF (EXISTS(SELECT TOP 1 1 FROM dbo.LeaveType  WITH(NOLOCK) WHERE LeaveTypeID = @LeaveTypeID AND IsALDeduction = 1) )
		SELECT @AnnualLeave = @TotalDays

	SELECT @AnnualLeave = ISNULL(@AnnualLeave ,0)

	IF EXISTS(SELECT TOP 1 1 FROM #LeaveRequestDetail )
		SELECT @KIVLeave = SUM(CASE cl.IsHalfDay WHEN 1 THEN 0.5 ELSE 1 END) 
		FROM #LeaveRequestDetail cl WITH(NOLOCK)
		INNER JOIN dbo.LeaveType l WITH(NOLOCK)  ON cl.LeaveType = l.LeaveTypeId
		WHERE  l.Code = 'KIV'
	ELSE IF (EXISTS(SELECT TOP 1 1 FROM dbo.LeaveType  WITH(NOLOCK) WHERE LeaveTypeID = @LeaveTypeID AND  Code = 'KIV') )
		SELECT @KIVLeave = @TotalDays

	SELECT @KIVLeave = ISNULL(@KIVLeave ,0)

	-- check the total day is not greater than annual leave balance
	IF (@AnnualLeave > 0)
	BEGIN
		SELECT @AnnualLeaveBalance= [dbo].[fnc_GetAnnualLeaveBalanceByDate] (@EmpID, @ToDate, NULL)
		IF (@AnnualLeave > @AnnualLeaveBalance)
		BEGIN
			SET @ErrCode =108
			SET @ErrMsg = 'You have not enough annual leave days to request'
			RETURN;
		END
	END	

	-- check the total day is not greater than KIV leave balance
	IF ( @KIVLeave > 0)
	BEGIN
		SELECT @KIVLeaveBalance = [dbo].[fnc_GetKIVBalance] (@EmpID, NULL)
		IF (@KIVLeave > @KIVLeaveBalance)
		BEGIN
			SET @ErrCode =111
			SET @ErrMsg = 'You have not enough Keep In View leave days to request'
			RETURN;
		END
	
	END	

	EXEC [dbo].WorkingTimeRequest_GetNewRefNoByRequestType 
		@RefNo =@RefNo output,
		@RequestType =6

	SET XACT_ABORT ON;
	BEGIN TRY
			INSERT INTO dbo.WorkingTimeRequest
			(	   [EmpId]
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
			VALUES
			(
				@EmpId, 
				@RequestType,	
				@Remark,
				CASE WHEN @IsTopManager=1 AND @VerifierId IS NOT NULL THEN 1 
					WHEN @IsTopManager=1 AND @ApproverId IS NOT NULL THEN 2 
					WHEN @IsTopManager=1 AND @ApproverId IS NULL THEN 4 
					ELSE 1 END, -- pending [CurrentStatus] as default
				@VerifierId,
				@ApproverId,
				CASE WHEN @IsTopManager=1 AND @VerifierId IS NULL THEN 2 ELSE 1 END, -- pending [VerifiedStatus] as default
				CASE WHEN @IsTopManager=1 AND @ApproverId IS NULL THEN 1 ELSE 1 END, -- pending [ApprovedStatus] as default
				1, -- pending [HRStatus] as default
				@CreatedDate, 
				@CreatedBy
			)

			SELECT @RequestId = cast(SCOPE_IDENTITY() as bigint)

			INSERT INTO [dbo].[LeaveRequest]
						([RequestId]
						,[EmpId]
						,[FromDate]
						,[ToDate]
						,[LeaveType]
						,[TotalDays]
						,[RefNo])
					VALUES
						(@RequestId,
						@EmpId,
						@FromDate,
						@ToDate,
						@LeaveTypeID,
						@TotalDays,
						@RefNo
						)
 
			 INSERT INTO [dbo].[LeaveRequestDetail]
					   ([RequestId]
					   ,[EmpId]
					   ,[OnDate]
					   ,[ShiftId]
					   ,[LeaveType]
					   ,[IsHalfDay]
					   ,[FromTime]
					   ,[ToTime]
					   ,WorkType)
			SELECT  @RequestId RequestId , @EmpId EmpId , rd.OnDate,ShiftID,LeaveType,IsHalfDay,FromTime,ToTime,WorkType
			FROM #LeaveRequestDetail rd

			EXEC [dbo].WorkingTimeRequest_SET_TmpLeaveBalance
				@Requestid = @Requestid,
				@EmpID = @EmpID
		
			EXEC [dbo].WorkingTimeRequest_SET_TmpKIVLeaveBalance
				@Requestid = @Requestid ,
				@EmpID = @EmpID ,
				@TotalDays =@KIVLeave

		IF (@ApproverId IS NULL AND @VerifierId IS NULL AND @IsTopManager = 1)
		BEGIN
				EXEC [dbo].[WorkingTimeRequest_ApproveRequest_6]
						@RequestID =@RequestID,
						@EmpID =@EmpID,
						@ProcessedBy =@EmpID,
						@ProcessedDate =@CreatedDate ,
						@RoleID =3,
						@ErrCode = @ErrCode  OUTPUT,
						@ErrMsg  = @ErrMsg   OUTPUT
		END
	
		EXEC [WorkingTimeRequest_WriteHistory] @RequestId

		SELECT @ErrCode=0;
	END TRY
	BEGIN CATCH
			SELECT @ErrCode = 100, @ErrMsg = N'Create leave request for an employee failed.'
			RETURN
	END CATCH

	DROP TABLE #LeaveRequestDetail
END