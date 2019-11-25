
CREATE PROCEDURE [dbo].[LeaveRequest_Approve_UpdateDiscipline] @RequestId BIGINT
	,@ModifiedBy INT
	,@DisciplineXml NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190426@Quinn.Vo
Task: ignored discipline score when approve leave request.
DB: HRM
Original: 

Revisions:
Param's Explanation:
    @DisciplineXml: 
        <Root>
            <Row WorkDate='' EmployeeId='' DisciplineTypeId='' IsIgnored='' IsConfirmed='' />
        </Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
	DECLARE @tblDiscipline TABLE (
		WorkDate DATE
		,EmployeeId INT
		,DisciplineTypeId INT
		,IsIgnored BIT
		)

	IF ISNULL(@DisciplineXml, '') <> ''
	BEGIN
		BEGIN TRY
			DECLARE @DocHandler INT

			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@DisciplineXml

			INSERT INTO @tblDiscipline (
				WorkDate
				,EmployeeId
				,DisciplineTypeId
				,IsIgnored
				)
			SELECT DISTINCT WorkDate
				,EmployeeId
				,DisciplineTypeId
				,IsIgnored
			FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (WorkDate DATE, EmployeeId INT, DisciplineTypeId INT, IsIgnored BIT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'Parameter invalid.'

			RETURN
		END CATCH
	END

	IF EXISTS (
			SELECT 1
			FROM @tblDiscipline
			)
	BEGIN
		INSERT INTO dbo.DisciplineRequestTrans (
			EmpId
			,WorkDate
			,RequestId
			,DisciplineTypeId
			,IsIgnored
			,IsOutOfControl
			,ModifiedDate
			)
		SELECT EmployeeId
			,WorkDate
			,@RequestId
			,DisciplineTypeId
			,IsIgnored
			,0
			,@ModifiedDate
		FROM @tblDiscipline

		INSERT INTO dbo.Discipline (
			EmpId
			,WorkDate
			,IsMissingCheckInOut
			,IsLateIn
			,IsEarlyOut
			,IsWorkingUnderTime
			,IsChangeShift
			,IsAbsent
			,IsUnpaidLeave
			,IsUnpaidLeaveAbsent
			,IsUnplannedLeave
			,IsWeekendLeave
			)
		SELECT DISTINCT EmployeeId
			,WorkDate
			,0 AS IsMissingCheckInOut
			,0 AS IsLateIn
			,0 AS IsEarlyOut
			,0 AS IsWorkingUnderTime
			,0 AS IsChangeShift
			,0 AS IsAbsent
			,0 AS IsUnpaidLeave
			,0 AS IsUnpaidLeaveAbsent
			,0 AS IsUnplannedLeave
			,0 AS IsWeekendLeave
		FROM @tblDiscipline AS ig
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.Discipline WITH (NOLOCK)
				WHERE EmpId = ig.EmployeeId
					AND WorkDate = ig.WorkDate
				)

		INSERT INTO dbo.DisciplineIgnore (
			EmpId
			,WorkDate
			,IsIgnoredMissingCheckInOut
			,IsIgnoredLateIn
			,IsIgnoredEarlyOut
			,IsIgnoredWorkingUnderTime
			,IsIgnoredChangeShift
			,IsIgnoredAbsent
			,IsIgnoredUnpaidLeave
			,IsIgnoredUnpaidLeaveAbsent
			,IsIgnoredUnplannedLeave
			,IsIgnoredWeekendLeave
			)
		SELECT DISTINCT EmployeeId
			,WorkDate
			,0 AS IsMissingCheckInOut
			,0 AS IsLateIn
			,0 AS IsEarlyOut
			,0 AS IsWorkingUnderTime
			,0 AS IsChangeShift
			,0 AS IsAbsent
			,0 AS IsUnpaidLeave
			,0 AS IsUnpaidLeaveAbsent
			,0 AS IsUnplannedLeave
			,0 AS IsWeekendLeave
		FROM @tblDiscipline AS ig
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.DisciplineIgnore WITH (NOLOCK)
				WHERE EmpId = ig.EmployeeId
					AND WorkDate = ig.WorkDate
				)

		UPDATE dis
		SET dis.IsAbsent = (CASE 
				WHEN ignore.DisciplineTypeId = 5 THEN 1
				ELSE dis.IsAbsent END
				)
			,dis.IsUnplannedLeave = (CASE 
				WHEN ignore.DisciplineTypeId = 9 THEN 1
				ELSE dis.IsUnplannedLeave END
				)
			,dis.IsUnpaidLeave = (CASE 
				WHEN ignore.DisciplineTypeId = 6 THEN 1
				ELSE dis.IsUnpaidLeave END
				)
			,dis.IsUnpaidLeaveAbsent = (CASE 
				WHEN ignore.DisciplineTypeId = 8 THEN 1
				ELSE dis.IsUnpaidLeaveAbsent END
				)
			,dis.IsDeleted = 0
			,dis.ModifiedBy = @ModifiedBy
			,dis.ModifiedDate = @ModifiedDate
		FROM dbo.Discipline AS dis WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @tblDiscipline AS ignore ON dis.WorkDate = ignore.WorkDate
			AND dis.EmpId = ignore.EmployeeId
			AND ignore.DisciplineTypeId <> 10

		UPDATE dis
		SET dis.IsWeekendLeave = (CASE 
				WHEN ignore.DisciplineTypeId = 10 THEN 1
				ELSE dis.IsWeekendLeave END
				)
			,dis.IsDeleted = 0
			,dis.ModifiedBy = @ModifiedBy
			,dis.ModifiedDate = @ModifiedDate
		FROM dbo.Discipline AS dis WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @tblDiscipline AS ignore ON dis.WorkDate = ignore.WorkDate
			AND dis.EmpId = ignore.EmployeeId
			AND ignore.DisciplineTypeId = 10

		UPDATE dis
		SET dis.IsIgnoredAbsent = (CASE 
				WHEN ignore.DisciplineTypeId = 5 THEN ignore.IsIgnored
				ELSE dis.IsIgnoredAbsent END
				)
			,dis.IsIgnoredUnplannedLeave = (CASE 
				WHEN ignore.DisciplineTypeId = 9 THEN ignore.IsIgnored
				ELSE dis.IsIgnoredUnplannedLeave END
				)
			,dis.IsIgnoredUnpaidLeave = (CASE 
				WHEN ignore.DisciplineTypeId = 6 THEN ignore.IsIgnored
				ELSE dis.IsIgnoredUnpaidLeave END
				)
			,dis.IsIgnoredUnpaidLeaveAbsent = (CASE 
				WHEN ignore.DisciplineTypeId = 8 THEN ignore.IsIgnored
				ELSE dis.IsIgnoredUnpaidLeaveAbsent END
				)
		FROM dbo.DisciplineIgnore AS dis WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @tblDiscipline AS ignore ON dis.WorkDate = ignore.WorkDate
			AND dis.EmpId = ignore.EmployeeId
			AND ignore.DisciplineTypeId <> 10

		UPDATE dis
		SET dis.IsIgnoredWeekendLeave = (CASE 
				WHEN ignore.DisciplineTypeId = 10 THEN ignore.IsIgnored
				ELSE dis.IsIgnoredWeekendLeave END
				)
		FROM dbo.DisciplineIgnore AS dis WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @tblDiscipline AS ignore ON dis.WorkDate = ignore.WorkDate
			AND dis.EmpId = ignore.EmployeeId
			AND ignore.DisciplineTypeId = 10
	END
END
