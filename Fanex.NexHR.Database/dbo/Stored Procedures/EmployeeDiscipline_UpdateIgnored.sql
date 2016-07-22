CREATE PROCEDURE [dbo].[EmployeeDiscipline_UpdateIgnored] @ModifiedBy INT
	,@IgnoredDetails NVARCHAR(MAX) = NULL
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
                IsIgnoredEarlyOut='{5}' IsIgnoredAbsent='{6}' IsIgnoredUnpaidLeave='{7}'
                IsIgnoredWorkingUnderTime='{7}' IsIgnoredUnpaidLeaveAbsent='{8}' IsConfirmed='{9}' ApproverSubtractScore='{10}'></Discipline>
        </ROOT>
*/
BEGIN
	SET NOCOUNT ON;
	
	CREATE TABLE #IgnoredDisciplineDetail (
		WorkDate DATE
		,EmployeeId INT
		,IsIgnoredChangeShift BIT
		,IsIgnoredMissingCheckInOut BIT
		,IsIgnoredLateIn BIT
		,IsIgnoredEarlyOut BIT
		,IsIgnoredAbsent BIT
		,IsIgnoredUnpaidLeave BIT
		,IsIgnoredWorkingUnderTime BIT
		,IsIgnoredUnpaidLeaveAbsent BIT
		,IsConfirmed BIT
		,ApproverSubtractScore DECIMAL(5, 2)
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
				,IsIgnoredChangeShift
				,IsIgnoredMissingCheckInOut
				,IsIgnoredLateIn
				,IsIgnoredEarlyOut
				,IsIgnoredAbsent
				,IsIgnoredUnpaidLeave
				,IsIgnoredWorkingUnderTime
				,IsIgnoredUnpaidLeaveAbsent
				,IsConfirmed
				,ApproverSubtractScore
			FROM OPENXML(@DocHandler, '/ROOT/Discipline', 1) WITH (WorkDate DATE, EmployeeId INT, IsIgnoredChangeShift BIT, IsIgnoredMissingCheckInOut BIT, IsIgnoredLateIn BIT, IsIgnoredEarlyOut BIT, IsIgnoredAbsent BIT, IsIgnoredUnpaidLeave BIT, IsIgnoredWorkingUnderTime BIT, IsIgnoredUnpaidLeaveAbsent BIT, IsConfirmed BIT, ApproverSubtractScore DECIMAL(5, 2)) AS Data

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

		DELETE ignore
		FROM #IgnoredDisciplineDetail AS ignore
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
				FROM #IgnoredDisciplineDetail
				)
		BEGIN
			UPDATE dis
			SET dis.DisciplineScore = ignore.ApproverSubtractScore
				,dis.IsIgnoredChangeShift = ignore.IsIgnoredChangeShift
				,dis.IsIgnoredMissingCheckInOut = ignore.IsIgnoredMissingCheckInOut
				,dis.IsIgnoredLateIn = ignore.IsIgnoredLateIn
				,dis.IsIgnoredEarlyOut = ignore.IsIgnoredEarlyOut
				,dis.IsIgnoredAbsent = ignore.IsIgnoredAbsent
				,dis.IsIgnoredUnpaidLeave = ignore.IsIgnoredUnpaidLeave
				,dis.IsIgnoredWorkingUndertime = ignore.IsIgnoredWorkingUnderTime
				,dis.IsIgnoredUnpaidLeaveAbsent = ignore.IsIgnoredUnpaidLeaveAbsent
				,dis.IsConfirmed = ignore.IsConfirmed
				,dis.ModifiedBy = @ModifiedBy
				,dis.ModifiedDate = GETDATE()
			FROM dbo.EmployeeDiscipline AS dis
				INNER JOIN #IgnoredDisciplineDetail AS ignore ON dis.WorkDate = ignore.WorkDate
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

	DROP TABLE #IgnoredDisciplineDetail
END