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
DB: SMSHRM
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

	CREATE TABLE #EmployeeDiscipline (
		WorkDate DATE
		,EmployeeId INT
		,NumberOfDiscipline INT
		,NumberOfDisciplineConfirmed INT
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

		DELETE
		FROM #IgnoredDisciplineDetail
		WHERE IsIgnored = 0
			AND IsConfirmed = 0

		IF EXISTS (
				SELECT 1
				FROM #IgnoredDisciplineDetail
				)
			AND @RoleId = 3 -- Approver
		BEGIN
			UPDATE dis
			SET dis.IsIgnoredChangeShift = (CASE 
					WHEN ignore.DisciplineTypeId = 1 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredChangeShift END
					)
				,dis.IsConfirmedChangeShift = (CASE 
					WHEN ignore.DisciplineTypeId = 1 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedChangeShift END
					)
				,dis.IsIgnoredMissingCheckInOut = (CASE 
					WHEN ignore.DisciplineTypeId = 2 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredMissingCheckInOut END
					)
				,dis.IsConfirmedMissingCheckInOut = (CASE 
					WHEN ignore.DisciplineTypeId = 2 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedMissingCheckInOut END
					)
				,dis.IsIgnoredLateIn = (CASE 
					WHEN ignore.DisciplineTypeId = 3 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredLateIn END
					)
				,dis.IsConfirmedLateIn = (CASE 
					WHEN ignore.DisciplineTypeId = 3 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedLateIn END
					)
				,dis.IsIgnoredEarlyOut = (CASE 
					WHEN ignore.DisciplineTypeId = 4 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredEarlyOut END
					)
				,dis.IsConfirmedEarlyOut = (CASE 
					WHEN ignore.DisciplineTypeId = 4 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedEarlyOut END
					)
				,dis.IsIgnoredAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 5 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredAbsent END
					)
				,dis.IsConfirmedAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 5 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedAbsent END
					)
				,dis.IsIgnoredUnpaidLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 6 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredUnpaidLeave END
					)
				,dis.IsConfirmedUnpaidLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 6 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedUnpaidLeave END
					)
				,dis.IsIgnoredWorkingUnderTime = (CASE 
					WHEN ignore.DisciplineTypeId = 7 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredWorkingUnderTime END
					)
				,dis.IsConfirmedWorkingUnderTime = (CASE 
					WHEN ignore.DisciplineTypeId = 7 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedWorkingUnderTime END
					)
				,dis.IsIgnoredUnpaidLeaveAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 8 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredUnpaidLeaveAbsent END
					)
				,dis.IsConfirmedUnpaidLeaveAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 8 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedUnpaidLeaveAbsent END
					)
				,dis.ModifiedBy = @ModifiedBy
				,dis.ModifiedDate = GETDATE()
			FROM dbo.EmployeeDiscipline AS dis
				INNER JOIN #IgnoredDisciplineDetail AS ignore ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
				AND dis.DisciplineType = 1

			DELETE ignore
			FROM #IgnoredDisciplineDetail AS ignore
				INNER JOIN dbo.EmployeeDiscipline AS dis ON ignore.WorkDate = dis.WorkDate
				AND ignore.EmployeeId = dis.EmpId
			WHERE dis.DisciplineType = 1

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
				,IsUnpaidLeave
				,IsIgnoredUnpaidLeave
				,IsConfirmedUnpaidLeave
				,IsWorkingUndertime
				,IsIgnoredWorkingUnderTime
				,IsConfirmedWorkingUnderTime
				,IsUnpaidLeaveAbsent
				,IsIgnoredUnpaidLeaveAbsent
				,IsConfirmedUnpaidLeaveAbsent
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
				,0 AS IsUnpaidLeave
				,0 AS IsIgnoredUnpaidLeave
				,0 AS IsConfirmedUnpaidLeave
				,0 AS IsWorkingUnderTime
				,0 AS IsIgnoredWorkingUnderTime
				,0 AS IsConfirmedWorkingUnderTime
				,0 AS IsUnpaidLeaveAbsent
				,0 AS IsIgnoredUnpaidLeaveAbsent
				,0 AS IsConfirmedUnpaidLeaveAbsent
				,@ModifiedBy
				,GETDATE()
			FROM #IgnoredDisciplineDetail

			UPDATE dis
			SET dis.IsIgnoredChangeShift = (CASE 
					WHEN ignore.DisciplineTypeId = 1 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredChangeShift END
					)
				,dis.IsConfirmedChangeShift = (CASE 
					WHEN ignore.DisciplineTypeId = 1 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedChangeShift END
					)
				,dis.IsIgnoredMissingCheckInOut = (CASE 
					WHEN ignore.DisciplineTypeId = 2 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredMissingCheckInOut END
					)
				,dis.IsConfirmedMissingCheckInOut = (CASE 
					WHEN ignore.DisciplineTypeId = 2 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedMissingCheckInOut END
					)
				,dis.IsIgnoredLateIn = (CASE 
					WHEN ignore.DisciplineTypeId = 3 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredLateIn END
					)
				,dis.IsConfirmedLateIn = (CASE 
					WHEN ignore.DisciplineTypeId = 3 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedLateIn END
					)
				,dis.IsIgnoredEarlyOut = (CASE 
					WHEN ignore.DisciplineTypeId = 4 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredEarlyOut END
					)
				,dis.IsConfirmedEarlyOut = (CASE 
					WHEN ignore.DisciplineTypeId = 4 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedEarlyOut END
					)
				,dis.IsIgnoredAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 5 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredAbsent END
					)
				,dis.IsConfirmedAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 5 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedAbsent END
					)
				,dis.IsIgnoredUnpaidLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 6 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredUnpaidLeave END
					)
				,dis.IsConfirmedUnpaidLeave = (CASE 
					WHEN ignore.DisciplineTypeId = 6 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedUnpaidLeave END
					)
				,dis.IsIgnoredWorkingUnderTime = (CASE 
					WHEN ignore.DisciplineTypeId = 7 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredWorkingUnderTime END
					)
				,dis.IsConfirmedWorkingUnderTime = (CASE 
					WHEN ignore.DisciplineTypeId = 7 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedWorkingUnderTime END
					)
				,dis.IsIgnoredUnpaidLeaveAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 8 THEN ignore.IsIgnored
					ELSE dis.IsIgnoredUnpaidLeaveAbsent END
					)
				,dis.IsConfirmedUnpaidLeaveAbsent = (CASE 
					WHEN ignore.DisciplineTypeId = 8 THEN ignore.IsConfirmed
					ELSE dis.IsConfirmedUnpaidLeaveAbsent END
					)
			FROM dbo.EmployeeDiscipline AS dis
				INNER JOIN #IgnoredDisciplineDetail AS ignore ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
				AND dis.DisciplineType = 1

			INSERT INTO #EmployeeDiscipline
			SELECT DISTINCT dis.WorkDate
				,dis.EmpId
				,(
					CAST((CASE 
							WHEN ISNULL(dis.IsChangeShift, '') = '' THEN 0
							ELSE dis.IsChangeShift END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsMissingCheckInOut, '') = '' THEN 0
							ELSE dis.IsMissingCheckInOut END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsLateIn, '') = '' THEN 0
							ELSE dis.IsLateIn END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsEarlyOut, '') = '' THEN 0
							ELSE dis.IsEarlyOut END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsAbsent, '') = '' THEN 0
							ELSE dis.IsAbsent END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsUnpaidLeave, '') = '' THEN 0
							ELSE dis.IsUnpaidLeave END
							) AS INT)
					) AS NumberOfDiscipline
				,(
					CAST((CASE 
							WHEN ISNULL(dis.IsConfirmedChangeShift, '') = '' THEN 0
							ELSE dis.IsConfirmedChangeShift END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsConfirmedMissingCheckInOut, '') = '' THEN 0
							ELSE dis.IsConfirmedMissingCheckInOut END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsConfirmedLateIn, '') = '' THEN 0
							ELSE dis.IsConfirmedLateIn END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsConfirmedEarlyOut, '') = '' THEN 0
							ELSE dis.IsConfirmedEarlyOut END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsConfirmedAbsent, '') = '' THEN 0
							ELSE dis.IsConfirmedAbsent END
							) AS INT) + CAST((CASE 
							WHEN ISNULL(dis.IsConfirmedUnpaidLeave, '') = '' THEN 0
							ELSE dis.IsConfirmedUnpaidLeave END
							) AS INT)
					) AS NumberOfDisciplineConfirmed
			FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
				INNER JOIN #IgnoredDisciplineDetail AS ignore ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
				AND dis.DisciplineType = 1

			UPDATE dis
			SET dis.IsConfirmed = (CASE 
					WHEN dis1.NumberOfDiscipline = dis1.NumberOfDisciplineConfirmed THEN 1
					ELSE 0 END
					)
			FROM dbo.EmployeeDiscipline AS dis
				INNER JOIN #EmployeeDiscipline AS dis1 ON dis.WorkDate = dis1.WorkDate
				AND dis.EmpId = dis1.EmployeeId
				AND dis.DisciplineType = 1
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Ignore discipline score unsuccessfully.'
	END CATCH

	DROP TABLE #IgnoredDisciplineDetail

	DROP TABLE #EmployeeDiscipline
END
