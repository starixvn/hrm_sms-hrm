
CREATE PROCEDURE [dbo].[LeaveRequest_Reset_UpdateDiscipline] @RequestId BIGINT
	,@ModifiedBy INT
	,@RoleId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190426@Quinn.Vo
Task: Reset ignore dicipline core
DB: SMSHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #DisciplineDetail(
		EmployeeId INT
		,WorkDate DATETIME
		,IsOffOrHoliday BIT
		,ScheduleStatus NVARCHAR(MAX)
		,IsUnpaidLeaveAbsent BIT
		,IsUnpaidLeave BIT
		,IsAbsent BIT
		,IsUnplannedLeave BIT
		,IsWeekendLeave BIT
	)

	INSERT INTO #DisciplineDetail(WorkDate
			,IsOffOrHoliday
			,ScheduleStatus
			,IsUnpaidLeaveAbsent
			,IsUnpaidLeave
			,IsAbsent
			,IsUnplannedLeave
			,IsWeekendLeave)
	SELECT WorkDate
			,IsOffOrHoliday
			,ScheduleStatus
			,IsUnpaidLeaveAbsent
			,IsUnpaidLeave
			,IsAbsent
			,IsUnplannedLeave
			,IsWeekendLeave
	FROM dbo.fnc_GetDisciplineDetailByLeaveRequestId(@RequestId)

	BEGIN TRY
		BEGIN TRANSACTION

		IF @RoleId IN (2, 3, 6) -- verifier/approver/hr
		BEGIN
			UPDATE dis
			SET 
				dis.IsIgnoredAbsent = (CASE 
					WHEN r.IsAbsent = 1 THEN 0
					ELSE dis.IsIgnoredAbsent END
					)
				,dis.IsConfirmedAbsent = (CASE 
					WHEN r.IsAbsent = 1 THEN 0
					ELSE dis.IsConfirmedAbsent END
					)
				,dis.IsIgnoredUnpaidLeave = (CASE 
					WHEN r.IsUnpaidLeave = 1 THEN 0
					ELSE dis.IsIgnoredUnpaidLeave END
					)
				,dis.IsConfirmedUnpaidLeave = (CASE 
					WHEN r.IsUnpaidLeave = 1 THEN 0
					ELSE dis.IsConfirmedUnpaidLeave END
					)
				,dis.IsIgnoredUnpaidLeaveAbsent = (CASE 
					WHEN r.IsUnpaidLeaveAbsent = 1 THEN 0
					ELSE dis.IsIgnoredUnpaidLeaveAbsent END
					)
				,dis.IsConfirmedUnpaidLeaveAbsent = (CASE 
					WHEN r.IsUnpaidLeaveAbsent = 1 THEN 0
					ELSE dis.IsConfirmedUnpaidLeaveAbsent END
					)
				,dis.IsIgnoredUnplannedLeave = (CASE 
					WHEN r.IsUnplannedLeave = 1 THEN 0
					ELSE dis.IsIgnoredUnplannedLeave END
					)
				,dis.IsConfirmedUnplannedLeave = (CASE 
					WHEN r.IsUnplannedLeave = 1 THEN 0
					ELSE dis.IsConfirmedUnplannedLeave END
					)
				,dis.IsIgnoredWeekendLeave = (CASE 
					WHEN r.IsWeekendLeave = 1 THEN 0
					ELSE dis.IsIgnoredWeekendLeave END
					)
				,dis.IsConfirmedWeekendLeave = (CASE 
					WHEN r.IsWeekendLeave = 1 THEN 0
					ELSE dis.IsConfirmedWeekendLeave END
					)
				,ModifiedBy = @ModifiedBy
				,ModifiedDate = GETDATE()
			FROM dbo.EmployeeDiscipline AS dis WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #DisciplineDetail AS r ON dis.WorkDate = r.WorkDate
				AND dis.EmpId = r.EmployeeId
				AND dis.DisciplineType = 1
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Ignore discipline score unsuccessfully.'
	END CATCH

	DROP TABLE #DisciplineDetail
END