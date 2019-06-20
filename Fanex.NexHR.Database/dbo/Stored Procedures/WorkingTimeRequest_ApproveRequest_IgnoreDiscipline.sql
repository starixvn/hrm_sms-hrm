

CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_IgnoreDiscipline] @RequestId BIGINT
	,@RoleId INT
	,@ModifiedBy INT
	,@IgnoredDetails NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160509@Tim.Trinh
Task: ignored discipline score.
DB: HRM
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

	CREATE TABLE #IgnoredDisciplineDetail (
		WorkDate DATE
		,EmployeeId INT
		,DisciplineTypeId INT
		,IsIgnored BIT
		,IsConfirmed BIT
		)

	IF ISNULL(@IgnoredDetails, '') <> ''
	BEGIN
		BEGIN TRY
			DECLARE @DocHandler INT

			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@IgnoredDetails

			INSERT INTO #IgnoredDisciplineDetail
			SELECT DISTINCT WorkDate
				,EmployeeId
				,DisciplineTypeId
				,IsIgnored
				,IsConfirmed
			FROM OPENXML(@DocHandler, '/ROOT/Discipline', 1) WITH (WorkDate DATE, EmployeeId INT, DisciplineTypeId INT, IsIgnored BIT, IsConfirmed BIT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'Parameter invalid.'

			RETURN
		END CATCH
	END

	BEGIN TRY
		BEGIN TRANSACTION

		IF EXISTS (
				SELECT 1
				FROM #IgnoredDisciplineDetail
				)
			AND @RoleId = 3 -- Approver
		BEGIN
			INSERT INTO dbo.EmployeeDiscipline (
				WorkDate
				,EmpId
				,DisciplineType
				,IsChangeShift
				,IsIgnoredChangeShift
				,IsConfirmedChangeShift
				,IsMissingCheckInOut
				,IsIgnoredMissingCheckInOut
				,IsConfirmedMissingCheckInOut
				,IsLateIn
				,IsIgnoredLateIn
				,IsConfirmedLateIn
				,IsEarlyOut
				,IsIgnoredEarlyOut
				,IsConfirmedEarlyOut
				,IsAbsent
				,IsIgnoredAbsent
				,IsConfirmedAbsent
				,IsUnplannedLeave
				,IsIgnoredUnplannedLeave
				,IsConfirmedUnplannedLeave
				,IsUnpaidLeave
				,IsIgnoredUnpaidLeave
				,IsConfirmedUnpaidLeave
				,IsWorkingUndertime
				,IsIgnoredWorkingUnderTime
				,IsConfirmedWorkingUnderTime
				,IsUnpaidLeaveAbsent
				,IsIgnoredUnpaidLeaveAbsent
				,IsConfirmedUnpaidLeaveAbsent
				,IsWeekendLeave
				,IsIgnoredWeekendLeave
				,IsConfirmedWeekendLeave
				,ModifiedBy
				,ModifiedDate
				)
			SELECT DISTINCT WorkDate
				,EmployeeId
				,1 AS DisciplineType
				,0 AS IsChangeShift
				,0 AS IsIgnoredChangeShift
				,0 AS IsConfirmedChangeShift
				,0 AS IsMissingCheckInOut
				,0 AS IsIgnoredMissingCheckInOut
				,0 AS IsConfirmedMissingCheckInOut
				,0 AS IsLateIn
				,0 AS IsIgnoredLateIn
				,0 AS IsConfirmedLateIn
				,0 AS IsEarlyOut
				,0 AS IsIgnoredEarlyOut
				,0 AS IsConfirmedEarlyOut
				,0 AS IsAbsent
				,0 AS IsIgnoredAbsent
				,0 AS IsConfirmedAbsent
				,0 AS IsUnplannedLeave
				,0 AS IsIgnoredUnplannedLeave
				,0 AS IsConfirmedUnplannedLeave
				,0 AS IsUnpaidLeave
				,0 AS IsIgnoredUnpaidLeave
				,0 AS IsConfirmedUnpaidLeave
				,0 AS IsWorkingUnderTime
				,0 AS IsIgnoredWorkingUnderTime
				,0 AS IsConfirmedWorkingUnderTime
				,0 AS IsUnpaidLeaveAbsent
				,0 AS IsIgnoredUnpaidLeaveAbsent
				,0 AS IsConfirmedUnpaidLeaveAbsent
				,0 AS IsWeekendLeave
				,0 AS IsIgnoredWeekendLeave
				,0 AS IsConfirmedWeekendLeave
				,@ModifiedBy
				,GETDATE()
			FROM #IgnoredDisciplineDetail AS ig
			WHERE NOT EXISTS (
				SELECT 1 FROM dbo.EmployeeDiscipline WITH (NOLOCK) WHERE EmpId = ig.EmployeeId AND WorkDate = ig.WorkDate
			)

			UPDATE dis
			SET dis.IsChangeShift = (CASE 
					WHEN ignore.DisciplineTypeId = 1 THEN 1
					ELSE dis.IsChangeShift END
					)
				,dis.IsIgnoredChangeShift = (CASE 
					WHEN ignore.DisciplineTypeId = 1 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredChangeShift END
					)
				,dis.IsConfirmedChangeShift = (CASE 
					WHEN ignore.DisciplineTypeId = 1 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedChangeShift END
					)
				,dis.IsMissingCheckInOut = (CASE 
					WHEN ignore.DisciplineTypeId = 2 THEN 1
					ELSE dis.IsMissingCheckInOut END
					)
				,dis.IsIgnoredMissingCheckInOut = (CASE 
					WHEN ignore.DisciplineTypeId = 2 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredMissingCheckInOut END
					)
				,dis.IsConfirmedMissingCheckInOut = (CASE 
					WHEN ignore.DisciplineTypeId = 2 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedMissingCheckInOut END
					)
				,dis.IsLateIn = (CASE 
					WHEN ignore.DisciplineTypeId = 3 THEN 1
					ELSE dis.IsLateIn END
					)
				,dis.IsIgnoredLateIn = (CASE 
					WHEN ignore.DisciplineTypeId = 3 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredLateIn END
					)
				,dis.IsConfirmedLateIn = (CASE 
					WHEN ignore.DisciplineTypeId = 3 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedLateIn END
					)
				,dis.IsEarlyOut = (CASE 
					WHEN ignore.DisciplineTypeId = 4 THEN 1
					ELSE dis.IsEarlyOut END
					)
				,dis.IsIgnoredEarlyOut = (CASE 
					WHEN ignore.DisciplineTypeId = 4 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredEarlyOut END
					)
				,dis.IsConfirmedEarlyOut = (CASE 
					WHEN ignore.DisciplineTypeId = 4 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedEarlyOut END
					)
				,dis.IsAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 5 THEN 1
					ELSE dis.IsAbsent END
					)
				,dis.IsIgnoredAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 5 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredAbsent END
					)
				,dis.IsConfirmedAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 5 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedAbsent END
					)
				,dis.IsUnplannedLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 9 THEN 1
					ELSE dis.IsUnplannedLeave END
					)
				,dis.IsIgnoredUnplannedLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 9 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredUnplannedLeave END
					)
				,dis.IsConfirmedUnplannedLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 9 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedUnplannedLeave END
					)
				,dis.IsUnpaidLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 6 THEN 1
					ELSE dis.IsUnpaidLeave END
					)
				,dis.IsIgnoredUnpaidLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 6 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredUnpaidLeave END
					)
				,dis.IsConfirmedUnpaidLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 6 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedUnpaidLeave END
					)
				,dis.IsWorkingUnderTime = (CASE 
					WHEN ignore.DisciplineTypeId = 7 THEN 1
					ELSE dis.IsWorkingUnderTime END
					)
				,dis.IsIgnoredWorkingUnderTime = (CASE 
					WHEN ignore.DisciplineTypeId = 7 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredWorkingUnderTime END
					)
				,dis.IsConfirmedWorkingUnderTime = (CASE 
					WHEN ignore.DisciplineTypeId = 7 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedWorkingUnderTime END
					)
				,dis.IsUnpaidLeaveAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 8 THEN 1
					ELSE dis.IsUnpaidLeaveAbsent END
					)
				,dis.IsIgnoredUnpaidLeaveAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 8 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredUnpaidLeaveAbsent END
					)
				,dis.IsConfirmedUnpaidLeaveAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 8 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedUnpaidLeaveAbsent END
					)
				,dis.IsDeleted = 0
				,dis.ModifiedBy = @ModifiedBy
				,dis.ModifiedDate = GETDATE()
			FROM dbo.EmployeeDiscipline AS dis WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #IgnoredDisciplineDetail AS ignore ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
				AND dis.DisciplineType = 1
				AND ignore.DisciplineTypeId <> 10

			UPDATE dis
			SET
				dis.IsWeekendLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 10 THEN 1
					ELSE dis.IsWeekendLeave END
					)
				,dis.IsIgnoredWeekendLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 10 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredWeekendLeave END
					)
				,dis.IsConfirmedWeekendLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 10 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedWeekendLeave END
					)
				,dis.IsDeleted = 0
				,dis.ModifiedBy = @ModifiedBy
				,dis.ModifiedDate = GETDATE()
			FROM dbo.EmployeeDiscipline AS dis WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #IgnoredDisciplineDetail AS ignore ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
				AND dis.DisciplineType = 1
				AND ignore.DisciplineTypeId = 10
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Ignore discipline score unsuccessfully.'
	END CATCH

	DROP TABLE #IgnoredDisciplineDetail
END