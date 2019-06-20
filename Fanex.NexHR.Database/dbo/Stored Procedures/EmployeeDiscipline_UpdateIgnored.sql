
CREATE PROCEDURE [dbo].[EmployeeDiscipline_UpdateIgnored] @ModifiedBy INT
	,@IgnoredDetails NVARCHAR(MAX) = NULL
	,@HasUpdateOutOfControlReason BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160512@Tim.Trinh
Task: ignored discipline score.
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
    @IgnoredDetails: 
        <ROOT>
            <Discipline WorkDate='{0}' EmployeeId='{1}' IsIgnoredChangeShift='{2}' IsIgnoredMissingCheckInOut='{3}' IsIgnoredLateIn='{4}'
                IsIgnoredEarlyOut='{5}' IsIgnoredAbsent='{6}' IsIgnoredUnplannedLeave='{7}' IsIgnoredUnpaidLeave='{8}'
                IsIgnoredWorkingUnderTime='{9}' IsIgnoredUnpaidLeaveAbsent='{10}' IsIgnoredWeekendLeave='{11}'
                IsConfirmed='{12}' ApproverSubtractScore='{13}' IsOutOfControlReason='{14}'></Discipline>
        </ROOT>
*/
BEGIN
	SET NOCOUNT ON;

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
		,IsConfirmed BIT
		,ApproverSubtractScore DECIMAL(5, 2)
		,IsOutOfControlReason BIT
		)

	IF ISNULL(@IgnoredDetails, '') <> ''
	BEGIN
		BEGIN TRY
			DECLARE @DocHandler INT

			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@IgnoredDetails

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
				,IsConfirmed
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
				,IsConfirmed
				,ApproverSubtractScore
				,IsOutOfControlReason
			FROM OPENXML(@DocHandler, '/ROOT/Discipline', 1) 
			     WITH (WorkDate DATE,
			         EmployeeId INT,
			         IsIgnoredChangeShift BIT,
			         IsIgnoredMissingCheckInOut BIT,
			         IsIgnoredLateIn BIT,
			         IsIgnoredEarlyOut BIT,
			         IsIgnoredAbsent BIT,
			         IsIgnoredUnplannedLeave BIT,
			         IsIgnoredUnpaidLeave BIT,
			         IsIgnoredWorkingUnderTime BIT,
			         IsIgnoredUnpaidLeaveAbsent BIT,
			         IsIgnoredWeekendLeave BIT,
			         IsConfirmed BIT,
			         ApproverSubtractScore DECIMAL(5, 2),
					 IsOutOfControlReason BIT) AS Data

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

		IF(@HasUpdateOutOfControlReason = 1)
		BEGIN
			UPDATE dis
				SET dis.IsOutOfControlReason = ignore.IsOutOfControlReason
					,dis.ModifiedBy = @ModifiedBy
					,dis.ModifiedDate = GETDATE()
				FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
					INNER JOIN #tblIgnoredDisciplineDetail AS ignore WITH (NOLOCK) ON dis.WorkDate = ignore.WorkDate
					AND dis.EmpId = ignore.EmployeeId
					AND dis.DisciplineType = 1
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
				WHERE empS.OwnerId = @ModifiedBy
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType = 8 -- edit discipline
					AND empS.EmpId <> @ModifiedBy
					AND empS.IsDeleted = 0
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE gr.GroupId = grS.GroupId
					AND grS.OwnerId = @ModifiedBy
					AND grS.ActionType = 8 -- edit discipline
					-- AND emp.EmpId <> @ModifiedBy
					AND grS.IsDeleted = 0
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ModifiedBy
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
				,dis.IsIgnoredChangeShift = ignore.IsIgnoredChangeShift
				,dis.IsIgnoredMissingCheckInOut = ignore.IsIgnoredMissingCheckInOut
				,dis.IsIgnoredLateIn = ignore.IsIgnoredLateIn
				,dis.IsIgnoredEarlyOut = ignore.IsIgnoredEarlyOut
				,dis.IsIgnoredAbsent = ignore.IsIgnoredAbsent
				,dis.IsIgnoredUnplannedLeave = ignore.IsIgnoredUnplannedLeave
				,dis.IsIgnoredUnpaidLeave = ignore.IsIgnoredUnpaidLeave
				,dis.IsIgnoredWorkingUndertime = ignore.IsIgnoredWorkingUnderTime
				,dis.IsIgnoredUnpaidLeaveAbsent = ignore.IsIgnoredUnpaidLeaveAbsent
				,dis.IsIgnoreWeekendLeave = ignore.IsIgnoredWeekendLeave
				,dis.IsConfirmed = ignore.IsConfirmed
				,dis.IsOutOfControlReason = ignore.IsOutOfControlReason
				,dis.ModifiedBy = @ModifiedBy
				,dis.ModifiedDate = GETDATE()
			FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
				INNER JOIN #tblIgnoredDisciplineDetail AS ignore WITH (NOLOCK) ON dis.WorkDate = ignore.WorkDate
				AND dis.EmpId = ignore.EmployeeId
				AND dis.DisciplineType = 1
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Ignored discipline score unsuccessfully.'
	END CATCH

	DROP TABLE #tblIgnoredDisciplineDetail
END