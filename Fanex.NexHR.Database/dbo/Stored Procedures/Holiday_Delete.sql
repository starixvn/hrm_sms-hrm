---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
 
CREATE PROCEDURE [dbo].[Holiday_Delete]
	@Id int	,
	@ModifiedBy int,
	@ModifiedDate datetime,
	@ErrCode INT = 0 OUTPUT,
	@ErrMsg VARCHAR(200) = '' OUTPUT 	  	
AS

/*
Created: Tom.Vo@2013-10-23
Task: Delete Holidate by ID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @Fromdate datetime, @todate datetime

Select @fromdate = fromdate, @todate = todate
from dbo.Holiday
where Id=@id

-- update working calendar for employee tu turn off the isHoliday 
UPDATE dbo.EmployeeCalendar WITH (ROWLOCK,UPDLOCK)
Set IsHoliday = 0,
	ModifiedBy = @ModifiedBy,
	ModifiedDate = @ModifiedDate
WHERE WorkDate BETWEEN @FromDate AND @ToDate

-- update the attendance data

IF(@fromdate < GETDATE())
BEGIN
	EXEC EmployeeAttendance_ProcessingAndInsert null, @fromdate, @todate , 1 
END

-- delete the holiday
DELETE Holiday
WHERE Id=@id

IF @@ERROR <> 0  
	SELECT @ErrCode = 1, @ErrMsg = N'Delete Holiday failed.'
ELSE  
	SELECT @ErrCode = 0 ,@ErrMsg = ''