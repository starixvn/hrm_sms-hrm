
CREATE PROCEDURE [dbo].[EmployeeWorkFromHome_Insert] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@CreatedBy INT
	,@Remark NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210528@Quinn.Vo
Task: Insert Employee Work From Home.
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
			WHERE EmpId = @EmpId
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

	INSERT INTO dbo.EmployeeWorkFromHome (
		EmpId
		,FromDate
		,ToDate
		,Remark
		,ModifiedBy
		,ModifiedDate
		)
	VALUES (
		@EmpId
		,@FromDate
		,@ToDate
		,@Remark
		,@CreatedBy
		,GETDATE()
		)
END