
CREATE PROCEDURE [dbo].[TimeRecored_Insert]  
	-- Add the parameters for the stored procedure here
	@EmpId int,
	@RecordTime datetime,
	@InOutType bit
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160701@Quinn.Vo
Task: Insert Shift.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
	DECLARE @ToDate DATETIME

    INSERT INTO [dbo].[TimeRecord] (EmpId, RecordTime, InOutType, InsertDate)
    VALUES(@EmpId,@RecordTime,@InOutType,GETDATE())

	SET @FromDate = convert(varchar(10),dateadd(d,-2,@RecordTime) ,111)
	SET @ToDate = convert(varchar(10), @RecordTime ,111)

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 0
END


