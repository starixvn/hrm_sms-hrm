

CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveMultipleRequest] @Requests NVARCHAR(MAX) = NULL
	,@ActorId INT
	,@RoleId TINYINT
	,@Command TINYINT
	,@Comment NVARCHAR(500) = ''
	,@IsIgnoredViolate BIT = 0
	,@IsConfirmed BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Drake.Doi@2020-04-10
Task: approve request
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
	@Requests:
        <ROOT>
            <Row RequestId='' RequestTypeID=''></Row>
        </ROOT>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE
		-- request type
		@CheckInRequestTypeId TINYINT = 1
		,@CheckOutRequestTypeId TINYINT = 2
		,@LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4
		,@ChangeShiftRequestTypeId TINYINT = 5
		,@LeaveRequestTypeId TINYINT = 6
		,@KeepInViewRequestTypeId TINYINT = 7
		-- Discipline Type
		,@ChangeShift INT = 1
		,@MissingCheckInOut INT = 2
		,@LateIn INT = 3
		,@EarlyOut INT = 4
		,@Absent INT = 5
		,@UnpaidLeave INT = 6
		,@UnpaidLeaveAbsent INT = 8
		,@UnplannedLeave INT = 9
		,@WeekenLeave INT = 10

	CREATE TABLE #request (
		RequestId BIGINT
		,RequestTypeID TINYINT
		)

	IF ISNULL(@Requests, '') <> ''
	BEGIN
		BEGIN TRY
			DECLARE @DocHandler INT

			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@Requests

			INSERT INTO #request (
				RequestId
				,RequestTypeID
				)
			SELECT DISTINCT RequestId
				,RequestTypeID
			FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (RequestId BIGINT, RequestTypeID TINYINT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'Parameter invalid.'

			RETURN
		END CATCH
	END
	ELSE
	BEGIN
		RETURN
	END

	DECLARE @RequestID BIGINT
		,@RequestTypeID TINYINT

	CREATE TABLE #Disciplines (
		WorkDate DATE
		,EmployeeId INT
		,DisciplineTypeId INT
		,IsIgnored VARCHAR(20)
		,IsConfirmed VARCHAR(20)
		)

	WHILE (
			EXISTS (
				SELECT TOP (1) RequestID
				FROM #request
				)
			)
	BEGIN
		SELECT TOP (1) @RequestID = RequestID
			,@RequestTypeID = RequestTypeID
		FROM #request

		IF @RoleId = 3 -- Approve
		BEGIN
			IF @RequestTypeId = @CheckInRequestTypeId
				OR @RequestTypeId = @CheckOutRequestTypeId
			BEGIN
				INSERT INTO #Disciplines (
					WorkDate
					,EmployeeId
					,DisciplineTypeId
					,IsIgnored
					,IsConfirmed
					)
				SELECT cio.OnDate
					,emp.EmpId
					,drule.DisciplineTypeId
					,@IsIgnoredViolate
					,@IsConfirmed
				FROM dbo.CheckInOutRequest AS cio WITH (NOLOCK)
					INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = cio.EmpId
					INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
					INNER JOIN dbo.DisciplineGroupRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
					AND empsum.IsLocal = drule.IsLocal
					AND drule.DisciplineTypeId = @MissingCheckInOut
					AND drule.Value = 1
				WHERE cio.RequestId = @RequestID
			END
			ELSE IF @RequestTypeId = @LateInRequestTypeId
				OR @RequestTypeId = @EarlyOutRequestTypeId
			BEGIN
				INSERT INTO #Disciplines (
					WorkDate
					,EmployeeId
					,DisciplineTypeId
					,IsIgnored
					,IsConfirmed
					)
				SELECT lio.OnDate
					,emp.EmpId
					,drule.DisciplineTypeId
					,@IsIgnoredViolate
					,@IsConfirmed
				FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
					INNER JOIN dbo.LateInEarlyOutRequest AS lio WITH (NOLOCK) ON wkr.RequestId = lio.RequestId
					AND lio.RequestId = @RequestId
					INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON ca.EmpId = wkr.EmpId
					AND CAST(ca.WorkDate AS DATE) = CAST(lio.OnDate AS DATE)
					INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = wkr.EmpId
					INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
					INNER JOIN dbo.DisciplineGroupRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
					AND emps.IsLocal = drule.IsLocal
					AND drule.Value = 1
					AND (
						wkr.RequestType = @LateInRequestTypeId
						OR (
							wkr.RequestType = @EarlyOutRequestTypeId
							AND ISNULL(ca.IsOffDay, 0) = 0
							AND ISNULL(ca.IsHoliday, 0) = 0
							)
						)
					AND (
						(
							wkr.RequestType = @LateInRequestTypeId
							AND drule.DisciplineTypeId = @LateIn
							)
						OR (
							wkr.RequestType = @EarlyOutRequestTypeId
							AND drule.DisciplineTypeId = @EarlyOut
							)
						)
			END
			ELSE IF @RequestTypeId = @ChangeShiftRequestTypeId
			BEGIN
				INSERT INTO #Disciplines (
					WorkDate
					,EmployeeId
					,DisciplineTypeId
					,IsIgnored
					,IsConfirmed
					)
				SELECT csdt.OnDate
					,csdt.EmpId
					,@ChangeShift
					,@IsIgnoredViolate
					,@IsConfirmed
				FROM dbo.ChangeShiftRequest AS cs WITH (NOLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
					INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = cs.EmpId
					INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
					INNER JOIN dbo.DisciplineGroupRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
					AND empsum.IsLocal = drule.IsLocal
					AND drule.DisciplineTypeId = @ChangeShift
					AND drule.Value = 1
				WHERE csdt.RequestId = @RequestID
					AND cs.TypeId = 1 -- Change Shift
			END
			ELSE IF @RequestTypeId = @LeaveRequestTypeId
			BEGIN
				INSERT INTO #Disciplines (
					WorkDate
					,EmployeeId
					,DisciplineTypeId
					,IsIgnored
					,IsConfirmed
					)
				SELECT WorkDate
					,lr.EmpId
					,CASE 
						WHEN IsUnpaidLeaveAbsent = 1 THEN @UnpaidLeaveAbsent
						WHEN IsAbsent = 1 THEN @Absent
						WHEN IsUnplannedLeave = 1 THEN @UnplannedLeave
						WHEN IsUnpaidLeave = 1 THEN @UnpaidLeave
						WHEN IsWeekendLeave = 1 THEN @WeekenLeave
						END AS DisciplineTypeId
					,CASE @IsIgnoredViolate
						WHEN 1 THEN 'True'
						ELSE 'False'
						END AS IsIgnoredViolate
					,CASE @IsConfirmed
						WHEN 1 THEN 'True'
						ELSE 'False'
						END AS IsConfirmed
				FROM [dbo].[fnc_GetDisciplineDetailByLeaveRequestId](@RequestID)
					INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON lr.RequestId = @RequestID
				WHERE IsUnpaidLeaveAbsent = 1
					OR IsAbsent = 1
					OR IsUnplannedLeave = 1
					OR IsUnpaidLeave = 1
					OR IsWeekendLeave = 1
			END
		END

		DECLARE @DisciplineXml NVARCHAR(MAX) = (
				SELECT *
				FROM #Disciplines
				FOR XML RAW('Row')
					,ROOT('Root')
				)

		EXECUTE WorkingTimeRequest_ApproveRequest @RequestID
			,@ActorId
			,@RoleId
			,@Comment
			,@DisciplineXml

		DELETE #request
		WHERE RequestId = @RequestID

		DELETE #Disciplines
	END

	DROP TABLE #Disciplines

	DROP TABLE #request
END
