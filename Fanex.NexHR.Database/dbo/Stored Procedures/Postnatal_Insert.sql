
CREATE PROCEDURE [dbo].[Postnatal_Insert] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@WorkingHours TINYINT
	,@CreatedBy INT
	,@Remark NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Insert Postnatal.
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
			,@ErrMsg = N'The postnatal of this employee is overlapping.'

		RETURN
	END
	
	DECLARE @normalWorkingHours TINYINT
	SELECT @normalWorkingHours = ISNULL(WorkingHours, 0)
	FROM dbo.EmployeeSummary WITH (NOLOCK)
	WHERE EmpId = @EmpId

	INSERT INTO dbo.EmployeePostnatal (
		EmpId
		,FromDate
		,ToDate
		,WorkingHours
		,NormalWorkingHours
		,CreatedBy
		,CreatedDate
		,Remark
		)
	VALUES (
		@EmpId
		,@FromDate
		,@ToDate
		,@WorkingHours
		,@normalWorkingHours
		,@CreatedBy
		,GETDATE()
		,@Remark
		)
END
