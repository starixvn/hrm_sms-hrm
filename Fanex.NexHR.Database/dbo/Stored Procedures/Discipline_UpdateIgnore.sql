
CREATE PROCEDURE [dbo].[Discipline_UpdateIgnore] @ActorId INT
	,@DisciplineIgnoreXml NVARCHAR(MAX) = NULL
	,@IgnoreDetailXml NVARCHAR(MAX) = NULL
	,@HasUpdateOutOfControlReason BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20191204@Tim.Trinh
Task: update ignore
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
    @IgnoreDetailXml: 
        <Root>
            <Row WorkDate='{0}' EmployeeId='{1}' IsIgnoredChangeShift='{2}' IsIgnoredMissingCheckInOut='{3}' IsIgnoredLateIn='{4}'
                IsIgnoredEarlyOut='{5}' IsIgnoredAbsent='{6}' IsIgnoredUnplannedLeave='{7}' IsIgnoredUnpaidLeave='{8}'
                IsIgnoredWorkingUnderTime='{9}' IsIgnoredUnpaidLeaveAbsent='{10}' IsIgnoredWeekendLeave='{11}'
                ApproverSubtractScore='{12}' IsOutOfControlReason='{13}' />
        </Root>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()

	CREATE TABLE #tblIgnoredDisciplineDetail (
		WorkDate DATE
		,EmployeeId INT
		,IsIgnoredChangeShift BIT
		,IsIgnoredMissingCheckInOut BIT
		,IsIgnoredLateIn BIT
		,IsIgnoredEarlyOut BIT
		,IsIgnoredAbsent BIT
		,IsIgnoredUnplannedLeave BIT
		,IsIgnoredUnpaidLeave BIT
		,IsIgnoredWorkingUnderTime BIT
		,IsIgnoredUnpaidLeaveAbsent BIT
		,IsIgnoredWeekendLeave BIT
		,ApproverSubtractScore DECIMAL(5, 2)
		,IsOutOfControlReason BIT
		)

	IF ISNULL(@DisciplineIgnoreXml, '') <> ''
	BEGIN
		EXEC dbo.DisciplineIgnoreDetail_Update
			@ActorId = @ActorId
			,@DisciplineIgnoreXml = @DisciplineIgnoreXml
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END

	IF ISNULL(@IgnoreDetailXml, '') <> ''
	BEGIN
		BEGIN TRY
			DECLARE @DocHandler INT

			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@IgnoreDetailXml

			INSERT INTO #tblIgnoredDisciplineDetail (
				WorkDate
				,EmployeeId
				,IsIgnoredChangeShift
				,IsIgnoredMissingCheckInOut
				,IsIgnoredLateIn
				,IsIgnoredEarlyOut
				,IsIgnoredAbsent
				,IsIgnoredUnplannedLeave
				,IsIgnoredUnpaidLeave
				,IsIgnoredWorkingUnderTime
				,IsIgnoredUnpaidLeaveAbsent
				,IsIgnoredWeekendLeave
				,ApproverSubtractScore
				,IsOutOfControlReason
				)
			SELECT DISTINCT WorkDate
				,EmployeeId
				,IsIgnoredChangeShift
				,IsIgnoredMissingCheckInOut
				,IsIgnoredLateIn
				,IsIgnoredEarlyOut
				,IsIgnoredAbsent
				,IsIgnoredUnplannedLeave
				,IsIgnoredUnpaidLeave
				,IsIgnoredWorkingUnderTime
				,IsIgnoredUnpaidLeaveAbsent
				,IsIgnoredWeekendLeave
				,ApproverSubtractScore
				,IsOutOfControlReason
			FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (WorkDate DATE, EmployeeId INT, IsIgnoredChangeShift BIT, IsIgnoredMissingCheckInOut BIT, IsIgnoredLateIn BIT, IsIgnoredEarlyOut BIT, IsIgnoredAbsent BIT, IsIgnoredUnplannedLeave BIT, IsIgnoredUnpaidLeave BIT, IsIgnoredWorkingUnderTime BIT, IsIgnoredUnpaidLeaveAbsent BIT, IsIgnoredWeekendLeave BIT, ApproverSubtractScore DECIMAL(5, 2), IsOutOfControlReason BIT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'Parameter invalid.'

			RETURN
		END CATCH
	END

	BEGIN TRY
		IF @HasUpdateOutOfControlReason = 1
		BEGIN
			UPDATE dis
			SET dis.IsOutOfControlReason = ignore.IsOutOfControlReason
				,dis.ModifiedBy = @ActorId
				,dis.ModifiedDate = @ModifiedDate
			FROM dbo.Discipline AS dis WITH (NOLOCK)
				INNER JOIN #tblIgnoredDisciplineDetail AS ignore WITH (NOLOCK) ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
		END

		DELETE ignore
		FROM #tblIgnoredDisciplineDetail AS ignore WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON ignore.EmployeeId = emp.EmpId
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
			LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @ActorId
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType = 8 -- edit discipline
					AND empS.EmpId <> @ActorId
					AND empS.IsDeleted = 0
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE gr.GroupId = grS.GroupId
					AND grS.OwnerId = @ActorId
					AND grS.ActionType = 8 -- edit discipline
					-- AND emp.EmpId <> @ActorId
					AND grS.IsDeleted = 0
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ActorId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType = 8
							AND empS.IsDeleted = 0
						)
				)

		IF EXISTS (
				SELECT 1
				FROM #tblIgnoredDisciplineDetail WITH (NOLOCK)
				)
		BEGIN
			UPDATE dis
			SET dis.DisciplineScore = ignore.ApproverSubtractScore
				,dis.IsOutOfControlReason = ignore.IsOutOfControlReason
				,dis.ModifiedBy = @ActorId
				,dis.ModifiedDate = @ModifiedDate
			FROM dbo.Discipline AS dis WITH (NOLOCK)
				INNER JOIN #tblIgnoredDisciplineDetail AS ignore WITH (NOLOCK) ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId

			UPDATE dis
			SET dis.IsIgnoredChangeShift = ignore.IsIgnoredChangeShift
				,dis.IsIgnoredMissingCheckInOut = ignore.IsIgnoredMissingCheckInOut
				,dis.IsIgnoredLateIn = ignore.IsIgnoredLateIn
				,dis.IsIgnoredEarlyOut = ignore.IsIgnoredEarlyOut
				,dis.IsIgnoredAbsent = ignore.IsIgnoredAbsent
				,dis.IsIgnoredUnplannedLeave = ignore.IsIgnoredUnplannedLeave
				,dis.IsIgnoredUnpaidLeave = ignore.IsIgnoredUnpaidLeave
				,dis.IsIgnoredWorkingUndertime = ignore.IsIgnoredWorkingUnderTime
				,dis.IsIgnoredUnpaidLeaveAbsent = ignore.IsIgnoredUnpaidLeaveAbsent
				,dis.IsIgnoredWeekendLeave = ignore.IsIgnoredWeekendLeave
			FROM dbo.DisciplineIgnore AS dis WITH (NOLOCK)
				INNER JOIN #tblIgnoredDisciplineDetail AS ignore WITH (NOLOCK) ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
		END
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH

	DROP TABLE #tblIgnoredDisciplineDetail
END