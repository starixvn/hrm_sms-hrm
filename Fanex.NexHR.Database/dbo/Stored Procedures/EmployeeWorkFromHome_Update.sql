
CREATE PROCEDURE [dbo].[EmployeeWorkFromHome_Update] @EmployeeWorkFromHomeId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Remark NVARCHAR(500) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210528@Quinn.Vo
Task: Update Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome WITH (NOLOCK)
			WHERE EmployeeWorkFromHomeId <> @EmployeeWorkFromHomeId
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
			,@ErrMsg = N'The WFH of this employee is overlapping.'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome WITH (NOLOCK)
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH of this employee does not exists'

		RETURN
	END

	UPDATE dbo.EmployeeWorkFromHome WITH (ROWLOCK, UPDLOCK)
	SET EmpId = @EmpId
		,FromDate = @FromDate
		,ToDate = @ToDate
		,Remark = @Remark
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

END