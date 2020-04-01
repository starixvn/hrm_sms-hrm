
CREATE PROCEDURE [dbo].[ChangeShiftRequest_ResetCalendar] @RequestId BIGINT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@20200303
Task: reset calendar
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		,@TypeId TINYINT

	SELECT @TypeId = TypeId
	FROM dbo.ChangeShiftRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @TypeId = 1
	BEGIN
		UPDATE empca
		SET empca.IsOffDay = 0
			,empca.ChangeShiftId = NULL
			,empca.ShiftId = csdt.FromShift
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
			AND empca.EmpId = csdt.EmpId
			AND csdt.ToShift = - 1
			AND csdt.RequestId = @RequestId

		UPDATE empca
		SET empca.ChangeShiftId = NULL
			,empca.ShiftId = NULL
			,empca.IsOffDay = 1
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
			AND empca.EmpId = csdt.EmpId
			AND csdt.RequestId = @RequestId
			AND csdt.ToShift <> - 1

		UPDATE empca
		SET empca.ChangeShiftId = NULL
			,empca.ShiftId = empca.ChangeShiftId
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
			AND empca.EmpId = csdt.EmpId
			AND csdt.RequestId = @RequestId
			AND csdt.ToShift <> - 1
	END
	ELSE
	BEGIN
		UPDATE empca
		SET empca.ShiftId = NULL
			,empca.IsOffDay = 1
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON empca.EmpId = cs.EmpId
			AND cs.RequestId = @RequestId
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND empca.WorkDate = csdt.OnDate

		UPDATE empca
		SET empca.IsOffDay = 0
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON empca.EmpId = cs.EmpId
			AND cs.RequestId = @RequestId
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND empca.WorkDate = csdt.ToDate
			AND csdt.RequestId = @RequestId
		WHERE empca.IsOffDay = 1
	END
END
