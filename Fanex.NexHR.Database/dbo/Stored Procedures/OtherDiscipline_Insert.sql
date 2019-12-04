
CREATE PROCEDURE [dbo].[OtherDiscipline_Insert] @EmpId INT
	,@WorkDate DATE
	,@DisciplineTypeId SMALLINT
	,@Score DECIMAL(5, 2)
	,@Comment NVARCHAR(500) = NULL
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20191204@Tim.Trinh
Task: insert other discipline.
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.OtherDiscipline WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND WorkDate = @WorkDate
				AND DisciplineTypeId = @DisciplineTypeId
				AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This violation has already existed for employee in violation date.'

		RETURN
	END

	BEGIN TRY
		DELETE
		FROM dbo.OtherDiscipline
		WHERE EmpId = @EmpId
			AND WorkDate = @WorkDate
			AND DisciplineTypeId = @DisciplineTypeId
			AND IsDeleted = 1

		INSERT INTO dbo.OtherDiscipline (
			WorkDate
			,EmpId
			,DisciplineTypeId
			,Score
			,Comment
			,IsDeleted
			,ModifiedBy
			,ModifiedDate
			)
		VALUES (
			@WorkDate
			,@EmpId
			,@DisciplineTypeId
			,@Score
			,@Comment
			,0
			,@ActorId
			,GETDATE()
			)
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert other violation unsuccessfully.'
	END CATCH
END
