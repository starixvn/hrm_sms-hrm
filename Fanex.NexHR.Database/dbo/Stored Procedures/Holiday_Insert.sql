---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

  
CREATE PROCEDURE [dbo].[Holiday_Insert]  
 @HolidayName nvarchar(50),  
 @FromDate datetime,  
 @ToDate datetime ,
 @ModifiedBy int,
 @ModifiedDate datetime 	 
AS  
 /*
Created: Tom.Vo@2013-10-23
Task: Insert Holidate
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
  
INSERT INTO [dbo].[Holiday]  
		(HolidayName,  
		FromDate,  
		ToDate)  
VALUES  
		(@HolidayName,  
		@FromDate,  
		@ToDate  
		)  

DECLARE @id int  
  
IF @@ERROR <> 0  
	SET @ID = 0  
ELSE  
	SET @id = cast(SCOPE_IDENTITY() as int)  
  
-- update working calendar for employee  
UPDATE dbo.EmployeeCalendar  WITH(ROWLOCK,UPDLOCK)
SET IsHoliday = 1,
	ModifiedBy = @ModifiedBy,
	ModifiedDate = @ModifiedDate
WHERE WorkDate BETWEEN @FromDate AND @ToDate  
  
-- update attendate record 
IF(@fromdate < GETDATE())
BEGIN
	EXEC EmployeeAttendance_ProcessingAndInsert null, @fromdate, @todate , 1 
END
SELECT @id
