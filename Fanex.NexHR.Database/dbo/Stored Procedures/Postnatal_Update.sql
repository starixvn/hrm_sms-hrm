
CREATE PROCEDURE [dbo].[Postnatal_Update] @PostnatalId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@WorkingHours TINYINT
	,@ModifiedBy INT
	,@Remark NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Update Postnatal.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeePostnatal WITH (NOLOCK)
			WHERE PostnatalId <> @PostnatalId
				AND EmpId = @EmpId
				AND (
					@FromDate BETWEEN FromDate
						AND ToDate
					OR @ToDate BETWEEN FromDate
						AND ToDate
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee is overlapping.'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeePostnatal WITH (NOLOCK)
			WHERE PostnatalId = @PostnatalId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee does not exists'

		RETURN
	END

	DECLARE @today DATE = GETDATE()
		,@oldEmpId INT
		,@oldWorkingHours TINYINT
		,@oldFromDate DATE
		,@oldToDate DATE

	SELECT @oldEmpId = EmpId
		,@oldWorkingHours = WorkingHours
		,@oldFromDate = FromDate
		,@oldToDate = ToDate
	FROM dbo.EmployeePostnatal WITH (NOLOCK)
	WHERE PostnatalId = @PostnatalId

	UPDATE dbo.EmployeePostnatal WITH (ROWLOCK, UPDLOCK)
	SET EmpId = @EmpId
		,FromDate = @FromDate
		,ToDate = @ToDate
		,WorkingHours = @WorkingHours
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
		,Remark = @Remark
	WHERE PostnatalId = @PostnatalId

	IF @oldEmpId <> @EmpId
		OR @oldWorkingHours <> @WorkingHours
		OR @oldFromDate <> @FromDate
		OR @oldToDate <> @ToDate
	BEGIN
		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate

		IF @oldEmpId <> @EmpId
		BEGIN
			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @oldEmpId
				,@oldFromDate
				,@oldToDate
		END
	END
END
