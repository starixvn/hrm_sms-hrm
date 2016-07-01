CREATE PROCEDURE [dbo].[DBA_Job_EmployeeAttendance_Insert] @ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @empid INT
		,@fromdate DATETIME
		,@todate DATETIME

	SELECT @fromdate = GETDATE() - 1
		,@todate = GETDATE()

	SET @fromdate = convert(VARCHAR(10), @fromdate, 111)
	SET @todate = convert(VARCHAR(10), @todate, 111)

	--set @empid=29
	EXEC dbo.[EmployeeAttendance_ProcessingAndInsert] @empid = @empid
		,@fromtime = @fromdate
		,@totime = @todate
		,@onlyUpdate = 0

	IF @@ERROR <> 0
		SELECT @ErrCode = 100
			,@ErrMsg = N'Create Default Calendar for employee failed'
	ELSE
		SELECT @ErrCode = 0
			,@ErrMsg = ''
END