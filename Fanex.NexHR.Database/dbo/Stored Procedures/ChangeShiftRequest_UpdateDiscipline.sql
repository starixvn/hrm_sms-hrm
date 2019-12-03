
CREATE PROCEDURE [dbo].[ChangeShiftRequest_UpdateDiscipline] @RequestId BIGINT
	,@ActorId INT
	,@RequestStatusId TINYINT
	,@DisciplineXml NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20191202@Tim.Trinh
Task: update discipline.
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
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		-- request status
		,@PendingStatusId TINYINT = 1
		,@NotVerifiedStatusId TINYINT = 3
		,@RejectedStatusId TINYINT = 5
		,@VoidStatusId TINYINT = 7
	DECLARE @tblDiscipline TABLE (
		WorkDate DATE
		,EmpId INT
		,DisciplineTypeId INT
		,IsIgnored BIT
		,IsConfirmed BIT
		)

	-- reject request
	IF @RequestStatusId = @NotVerifiedStatusId
		OR @RequestStatusId = @RejectedStatusId
		OR @RequestStatusId = @VoidStatusId
	BEGIN
		EXEC dbo.DisciplineRequestTrans_Delete @RequestId = @RequestId

		UPDATE dis
		SET dis.IsChangeShift = 0
			,dis.ModifiedBy = @ActorId
			,dis.ModifiedDate = @ModifiedDate
		FROM dbo.Discipline AS dis WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON cs.RequestId = @RequestId
			AND dis.EmpId = cs.EmpId
			AND cs.TypeId = 1 -- change shift
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND dis.WorkDate = csdt.OnDate
		WHERE EXISTS (
				SELECT 1
				FROM dbo.Employee AS emp WITH (NOLOCK)
					INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
					INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
					AND drule.DisciplineTypeId = 1
					AND empsum.IsLocal = drule.IsLocal
					AND drule.Value = 1
				WHERE dis.EmpId = emp.EmpId
				)

		UPDATE dis
		SET dis.IsIgnoredChangeShift = 0
		FROM dbo.DisciplineIgnore AS dis WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON cs.RequestId = @RequestId
			AND dis.EmpId = cs.EmpId
			AND cs.TypeId = 1 -- change shift
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND dis.WorkDate = csdt.OnDate
		WHERE EXISTS (
				SELECT 1
				FROM dbo.Employee AS emp WITH (NOLOCK)
					INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
					INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
					AND drule.DisciplineTypeId = 1
					AND empsum.IsLocal = drule.IsLocal
					AND drule.Value = 1
				WHERE dis.EmpId = emp.EmpId
				)

		RETURN
	END

	-- reset request
	IF @RequestStatusId = @PendingStatusId
	BEGIN
		EXEC dbo.DisciplineRequestTrans_Delete @RequestId = @RequestId

		UPDATE dis
		SET dis.IsChangeShift = 1
			,dis.IsDeleted = 0
			,dis.ModifiedBy = @ActorId
			,dis.ModifiedDate = @ModifiedDate
		FROM dbo.Discipline AS dis WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON cs.RequestId = @RequestId
			AND dis.EmpId = cs.EmpId
			AND cs.TypeId = 1 -- change shift
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND dis.WorkDate = csdt.OnDate
		WHERE EXISTS (
				SELECT 1
				FROM dbo.Employee AS emp WITH (NOLOCK)
					INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
					INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
					AND drule.DisciplineTypeId = 1
					AND empsum.IsLocal = drule.IsLocal
					AND drule.Value = 1
				WHERE dis.EmpId = emp.EmpId
				)

		RETURN
	END

	-- approve request
	IF ISNULL(@DisciplineXml, '') <> ''
	BEGIN
		BEGIN TRY
			DECLARE @DocHandler INT

			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@DisciplineXml

			INSERT INTO @tblDiscipline (
				WorkDate
				,EmpId
				,DisciplineTypeId
				,IsIgnored
				,IsConfirmed
				)
			SELECT DISTINCT WorkDate
				,EmployeeId
				,DisciplineTypeId
				,IsIgnored
				,IsConfirmed
			FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (WorkDate DATE, EmployeeId INT, DisciplineTypeId INT, IsIgnored BIT, IsConfirmed BIT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'Parameter invalid.'

			RETURN
		END CATCH
	END

	BEGIN TRY
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
				,ModifiedBy
				,ModifiedDate
				)
			SELECT EmpId
				,WorkDate
				,@RequestId
				,DisciplineTypeId
				,IsIgnored
				,0
				,@ActorId
				,@ModifiedDate
			FROM @tblDiscipline

			-- update
			UPDATE dis
			SET dis.IsChangeShift = 1
				,dis.IsDeleted = 0
				,dis.ModifiedBy = @ActorId
				,dis.ModifiedDate = @ModifiedDate
			FROM dbo.Discipline AS dis WITH (ROWLOCK, UPDLOCK)
				INNER JOIN @tblDiscipline AS tbl ON dis.WorkDate = tbl.WorkDate
				AND dis.EmpId = tbl.EmpId
				AND tbl.DisciplineTypeId = 1

			UPDATE dis
			SET dis.IsIgnoredChangeShift = tbl.IsIgnored
			FROM dbo.DisciplineIgnore AS dis WITH (ROWLOCK, UPDLOCK)
				INNER JOIN @tblDiscipline AS tbl ON dis.WorkDate = tbl.WorkDate
				AND dis.EmpId = tbl.EmpId
				AND tbl.DisciplineTypeId = 1

			-- insert
			INSERT INTO dbo.Discipline (
				EmpId
				,WorkDate
				,IsChangeShift
				,ModifiedBy
				,ModifiedDate
				)
			SELECT EmpId
				,WorkDate
				,1
				,@ActorId
				,@ModifiedDate
			FROM @tblDiscipline AS tbl
			WHERE tbl.DisciplineTypeId = 1
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.Discipline AS dis WITH (NOLOCK)
					WHERE dis.EmpId = tbl.EmpId
						AND dis.WorkDate = tbl.WorkDate
					)

			INSERT INTO dbo.DisciplineIgnore (
				EmpId
				,WorkDate
				,IsIgnoredChangeShift
				)
			SELECT EmpId
				,WorkDate
				,IsIgnored
			FROM @tblDiscipline AS tbl
			WHERE tbl.DisciplineTypeId = 1
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.DisciplineIgnore AS dis WITH (NOLOCK)
					WHERE dis.EmpId = tbl.EmpId
						AND dis.WorkDate = tbl.WorkDate
					)
		END
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update discipline unsuccessfully.'
	END CATCH
END
