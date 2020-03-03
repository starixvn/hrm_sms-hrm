
CREATE PROCEDURE [dbo].[Dashboard_GetMemberRequest] @EmployeeId INT
	,@Date DATE
AS
/*
Created: Tim.Trinh@2019-06-19
Task: show dashboard info for employee
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDate DATE = DATEADD(d, - 7, @Date)
		,@toDate DATE = DATEADD(d, 1, @Date)
		,@TotalOvertimePendingByVerifier INT
		,@TotalOvertimePendingByApprover INT
		-- role
		,@VerifierId TINYINT = 2
		,@ApproverId TINYINT = 3
		-- request type
		,@CheckInTypeId TINYINT = 1
		,@CheckOutTypeId TINYINT = 2
		,@LateInTypeId TINYINT = 3
		,@EarlyOutTypeId TINYINT = 4
		,@ChangeShiftTypeId TINYINT = 5
		,@LeaveTypeId TINYINT = 6
		,@KeepInViewTypeId TINYINT = 7
		,@OvertimeTypeId TINYINT = 8
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7
	DECLARE @tblrole TABLE (RoleId TINYINT)
	DECLARE @tblRequestType TABLE (
		Id TINYINT
		,[Name] NVARCHAR(100)
		)

	INSERT INTO @tblRequestType
	VALUES (
		@CheckInTypeId
		,'Check in/out'
		)
		,(
		@LateInTypeId
		,'Late In/Early Out'
		)
		,(
		@ChangeShiftTypeId
		,'Change Shift'
		)
		,(
		@LeaveTypeId
		,'Leave'
		)
		,(
		@KeepInViewTypeId
		,'Keep in view'
		)
		,(
		@OvertimeTypeId
		,'Overtime'
		)

	INSERT INTO @tblrole
	SELECT emprole.RoleId
	FROM dbo.EmployeeRole AS emprole WITH (NOLOCK)
		INNER JOIN dbo.LookupData AS lkdata WITH (NOLOCK) ON emprole.RoleId = lkdata.Value
		INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lk.Id = lkdata.LookupId
		AND lk.Code = 'SR' -- System Role
	WHERE emprole.EmployeeId = @EmployeeId
		AND emprole.RoleId IN (@VerifierId, @ApproverId)

	CREATE TABLE #tblData (
		RoleId TINYINT
		,RequestTypeId TINYINT
		,TotalPending INT
		,TotalApproved INT
		,TotalRejected INT
		,FromDate DATE
		,ToDate DATE
		)

	-- overtime
	SELECT @TotalOvertimePendingByVerifier = COUNT(1)
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE VerifierId = @EmployeeId
		AND CurrentStatus = @PendingStatusId
		AND (
			ISNULL(NormalHours, 0) <> 0
			OR ISNULL(OffHours, 0) <> 0
			OR ISNULL(PublicHours, 0) <> 0
			OR ISNULL(SpecialHours, 0) <> 0
			)

	SELECT @TotalOvertimePendingByApprover = COUNT(1)
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE ApproverId = @EmployeeId
		AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId)
		AND (
			ISNULL(NormalHours, 0) <> 0
			OR ISNULL(OffHours, 0) <> 0
			OR ISNULL(PublicHours, 0) <> 0
			OR ISNULL(SpecialHours, 0) <> 0
			)

	-- pending
	INSERT INTO #tblData (
		RoleId
		,RequestTypeId
		,TotalPending
		,TotalApproved
		,TotalRejected
		,FromDate
		,ToDate
		)
	SELECT CASE 
			WHEN VerifierId = @EmployeeId
				AND VerifierId <> ApproverId THEN @VerifierId
			WHEN ApproverId = @EmployeeId THEN @ApproverId
			END
		,CASE 
			WHEN RequestType IN (@CheckInTypeId, @CheckOutTypeId) THEN @CheckInTypeId
			WHEN RequestType IN (@LateInTypeId, @EarlyOutTypeId) THEN @LateInTypeId
			WHEN RequestType = @ChangeShiftTypeId THEN @ChangeShiftTypeId
			WHEN RequestType = @LeaveTypeId THEN @LeaveTypeId
			WHEN RequestType = @KeepInViewTypeId THEN @KeepInViewTypeId
			END
		,SUM(CASE 
				WHEN CASE 
						WHEN VerifierId = @EmployeeId
							AND CurrentStatus = @PendingStatusId THEN 1
						WHEN ApproverId = @EmployeeId
							AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
						ELSE 0
						END = 1 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN CASE 
						WHEN VerifierId = @EmployeeId
							AND CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
						WHEN ApproverId = @EmployeeId
							AND CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId) THEN 1
						ELSE 0
						END = 1 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN CASE 
						WHEN VerifierId = @EmployeeId
							AND CurrentStatus IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId) THEN 1
						WHEN ApproverId = @EmployeeId
							AND CurrentStatus IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId) THEN 1
						ELSE 0
						END = 1 THEN 1
				ELSE 0
				END)
		,MIN(CreatedDate) AS FromDate
		,MAX(CreatedDate) AS ToDate
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE (
			VerifierId = @EmployeeId
			OR ApproverId = @EmployeeId
			)
		AND CASE 
			WHEN VerifierId = @EmployeeId
				AND CurrentStatus = @PendingStatusId THEN 1
			WHEN ApproverId = @EmployeeId
				AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
			ELSE 0
			END = 1
	GROUP BY CASE 
			WHEN VerifierId = @EmployeeId
				AND VerifierId <> ApproverId THEN @VerifierId
			WHEN ApproverId = @EmployeeId THEN @ApproverId
			END
		,CASE 
			WHEN RequestType IN (@CheckInTypeId, @CheckOutTypeId) THEN @CheckInTypeId
			WHEN RequestType IN (@LateInTypeId, @EarlyOutTypeId) THEN @LateInTypeId
			WHEN RequestType = @ChangeShiftTypeId THEN @ChangeShiftTypeId
			WHEN RequestType = @LeaveTypeId THEN @LeaveTypeId
			WHEN RequestType = @KeepInViewTypeId THEN @KeepInViewTypeId
			END
	
	UNION
	SELECT @VerifierId
		,@OvertimeTypeId
		,@TotalOvertimePendingByVerifier
		,0
		,0
		,NULL
		,NULL

	-- approved
	INSERT INTO #tblData (
		RoleId
		,RequestTypeId
		,TotalPending
		,TotalApproved
		,TotalRejected
		)
	SELECT CASE 
			WHEN VerifierId = @EmployeeId
				AND VerifierId <> ApproverId THEN @VerifierId
			WHEN ApproverId = @EmployeeId THEN @ApproverId
			END
		,CASE 
			WHEN RequestType IN (@CheckInTypeId, @CheckOutTypeId) THEN @CheckInTypeId
			WHEN RequestType IN (@LateInTypeId, @EarlyOutTypeId) THEN @LateInTypeId
			WHEN RequestType = @ChangeShiftTypeId THEN @ChangeShiftTypeId
			WHEN RequestType = @LeaveTypeId THEN @LeaveTypeId
			WHEN RequestType = @KeepInViewTypeId THEN @KeepInViewTypeId
			END
		,SUM(CASE 
				WHEN CASE 
						WHEN VerifierId = @EmployeeId
							AND CurrentStatus = @PendingStatusId THEN 1
						WHEN ApproverId = @EmployeeId
							AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
						ELSE 0
						END = 1 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN CASE 
						WHEN VerifierId = @EmployeeId
							AND CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
						WHEN ApproverId = @EmployeeId
							AND CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId) THEN 1
						ELSE 0
						END = 1 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN CASE 
						WHEN VerifierId = @EmployeeId
							AND CurrentStatus IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId) THEN 1
						WHEN ApproverId = @EmployeeId
							AND CurrentStatus IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId) THEN 1
						ELSE 0
						END = 1 THEN 1
				ELSE 0
				END)
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE CreatedDate >= @fromdate
		AND CreatedDate < @toDate
		AND (
			VerifierId = @EmployeeId
			OR ApproverId = @EmployeeId
			)
		AND CASE 
			WHEN VerifierId = @EmployeeId
				AND CurrentStatus = @PendingStatusId THEN 1
			WHEN ApproverId = @EmployeeId
				AND CurrentStatus IN (@PendingStatusId, @VerifiedStatusId) THEN 1
			ELSE 0
			END = 0
	GROUP BY CASE 
			WHEN VerifierId = @EmployeeId
				AND VerifierId <> ApproverId THEN @VerifierId
			WHEN ApproverId = @EmployeeId THEN @ApproverId
			END
		,CASE 
			WHEN RequestType IN (@CheckInTypeId, @CheckOutTypeId) THEN @CheckInTypeId
			WHEN RequestType IN (@LateInTypeId, @EarlyOutTypeId) THEN @LateInTypeId
			WHEN RequestType = @ChangeShiftTypeId THEN @ChangeShiftTypeId
			WHEN RequestType = @LeaveTypeId THEN @LeaveTypeId
			WHEN RequestType = @KeepInViewTypeId THEN @KeepInViewTypeId
			END
	
	UNION
	SELECT @ApproverId
		,@OvertimeTypeId
		,@TotalOvertimePendingByApprover
		,0
		,0

	SELECT tblrole.RoleId
		,tblrole.RequestTypeId
		,tblrole.RequestType
		,SUM(TotalPending) AS TotalPending
		,SUM(TotalApproved) AS TotalApproved
		,SUM(TotalRejected) AS TotalRejected
		,MIN(data.FromDate) AS FromDate
		,MAX(data.ToDate) AS ToDate
	FROM (
		SELECT tblrole.RoleId
			,rqtype.Id AS RequestTypeId
			,rqtype.[Name] AS RequestType
		FROM @tblrole AS tblrole
			,@tblRequestType AS rqtype
		) AS tblrole
		LEFT JOIN #tblData AS data ON tblrole.RoleId = data.RoleId
		AND tblrole.RequestTypeId = data.RequestTypeId
	GROUP BY tblrole.RoleId
		,tblrole.RequestTypeId
		,tblrole.RequestType
	ORDER BY tblrole.RoleId
		,tblrole.RequestTypeId

	DROP TABLE #tblData
END
