---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[DBA_Job_EmployeeAttendance_Insert]
	@ErrCode int=0 OUTPUT,
	@ErrMsg varchar(200)='' OUTPUT
AS
SET NOCOUNT ON

DECLARE	@empid int,
	@fromdate datetime, 
	@todate datetime 

SELECT @fromdate = GETDATE()-1,@todate=GETDATE()

set @fromdate = convert(varchar(10),@fromdate,111)
set @todate = convert(varchar(10),@todate,111)
--set @empid=29

EXEC dbo.[EmployeeAttendance_ProcessingAndInsert]
		@empid =@empid, 
		@fromtime =@fromdate, 
		@totime =@todate,
		@onlyUpdate=0,
		@ErrCode =@ErrCode OUTPUT,@ErrMsg = @ErrMsg OUTPUT 
		,@Debug =1

 IF @@ERROR <> 0 
	SELECT @ErrCode=100,@ErrMsg=N'Create Default Calendar for employee failed'
ELSE 
	SELECT @ErrCode=0,@ErrMsg=''


