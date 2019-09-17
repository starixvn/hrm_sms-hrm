
CREATE PROCEDURE [dbo].[ChangeShiftRequest_VoidBySA] @RequestId BIGINT
	,@ProcessedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-09-17
Task: void change shift
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @StatusId TINYINT
		,@ProcessedDate DATE = GETDATE()
		,@FromDate DATE
		,@ToDate DATE
		,@EmpId INT
		,@TypeId TINYINT
		-- change shift type
		,@ChangeShift TINYINT = 1
		,@MoveOffDay TINYINT = 2
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @StatusId = wk.CurrentStatus
		,@FromDate = cs.FromDate
		,@ToDate = cs.ToDate
		,@EmpId = wk.EmpId
		,@TypeId = cs.TypeId
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON wk.RequestId = cs.RequestId
	WHERE wk.RequestId = @RequestId

	UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
	SET CurrentStatus = @VoidStatusId
		,ModifiedBy = @ProcessedBy
		,ModifiedDate = @ProcessedDate
	WHERE RequestId = @RequestId

	IF @StatusId IN (@ApprovedStatusId, @ProcessedStatusId)
	BEGIN
		IF @TypeId = @ChangeShift
		BEGIN
			UPDATE empca
			SET empca.IsOffDay = 0
				,empca.ChangeShiftId = NULL
				,empca.ShiftId = csdt.FromShift
				,empca.ModifiedBy = @ProcessedBy
				,empca.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
				AND empca.EmpId = csdt.EmpId
				AND csdt.ToShift = - 1
				AND csdt.RequestId = @RequestId
			WHERE empca.EmpId = @EmpId
				AND empca.ChangeShiftId IS NOT NULL

			UPDATE empca
			SET empca.ShiftId = NULL
				,empca.IsOffDay = 1
				,empca.ModifiedBy = @ProcessedBy
				,empca.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
				AND empca.EmpId = csdt.EmpId
				AND csdt.RequestId = @RequestId
				AND csdt.ToShift <> - 1
			WHERE empca.EmpId = @EmpId
				AND empca.ChangeShiftId IS NULL

			UPDATE empca
			SET empca.ChangeShiftId = NULL
				,empca.ShiftId = empca.ChangeShiftId
				,empca.ModifiedBy = @ProcessedBy
				,empca.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
				AND empca.EmpId = csdt.EmpId
				AND csdt.RequestId = @RequestId
				AND csdt.ToShift <> - 1
			WHERE empca.EmpId = @EmpId
				AND empca.ChangeShiftId IS NOT NULL
		END
		ELSE
		BEGIN
			UPDATE empca
			SET empca.IsOffDay = 1
				,empca.ModifiedBy = @ProcessedBy
				,empca.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
				AND empca.EmpId = csdt.EmpId
				AND csdt.RequestId = @RequestId
			WHERE empca.EmpId = @EmpId
				AND empca.IsOffDay = 0

			UPDATE empca
			SET empca.IsOffDay = 0
				,empca.ModifiedBy = @ProcessedBy
				,empca.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.ToDate
				AND empca.EmpId = csdt.EmpId
				AND csdt.RequestId = @RequestId
			WHERE empca.EmpId = @EmpId
				AND empca.IsOffDay = 1
		END

		EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate
			,1
	END

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void unsuccessfully.'
	END
END
