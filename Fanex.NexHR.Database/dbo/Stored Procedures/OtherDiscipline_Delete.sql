
CREATE PROCEDURE [dbo].[OtherDiscipline_Delete] @EmpId INT
	,@WorkDate DATE
	,@DisciplineTypeId SMALLINT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20191204@Tim.Trinh
Task: delete other discipline.
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	UPDATE dbo.OtherDiscipline WITH (ROWLOCK, UPDLOCK)
	SET IsDeleted = 1
		,ModifiedBy = @ActorId
		,ModifiedDate = GETDATE()
	WHERE EmpId = @EmpId
		AND WorkDate = @WorkDate
		AND DisciplineTypeId = @DisciplineTypeId
END
