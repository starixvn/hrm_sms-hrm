---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Holiday_Update]
	@Id int,
	@HolidayName nvarchar(50),	
	@FromDate datetime,
	@ToDate datetime,
	@ModifiedBy int,
	@ModifiedDate datetime 	
AS
/*
Created: Tom.Vo@2013-10-23
Task: update a holiday
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
SET NOCOUNT ON
SET XACT_ABORT ON;

DECLARE @OldFromdate datetime, @Oldtodate datetime

Select @OldFromdate = fromdate, @Oldtodate = todate
from dbo.Holiday WITH(NOLOCK)
where Id=@id

BEGIN TRANSACTION;

IF (@OldFromdate=@FromDate AND @ToDate = @Oldtodate)
begin
	UPDATE dbo.Holiday
	SET HolidayName = @HolidayName				
	WHERE ID=@ID	
end
else
begin
	-- update working calendar for employee tu turn off the isHoliday for previous  time range
	UPDATE dbo.EmployeeCalendar WITH(ROWLOCK,UPDLOCK)
	Set IsHoliday = 0
	WHERE WorkDate BETWEEN @OldFromdate AND @Oldtodate	

	-- just update attendate record from past to today
	-- EXEC EmployeeAttendance_ProcessingAndInsert null, @OldFromdate, @Oldtodate, 1

	-- update working calendar for employee tu turn on the isHoliday  for new range
	UPDATE dbo.EmployeeCalendar WITH(ROWLOCK,UPDLOCK)
	Set IsHoliday = 1,
		ModifiedBy = @ModifiedBy,
		ModifiedDate = @ModifiedDate
	WHERE WorkDate BETWEEN @FromDate AND @ToDate

	IF(@fromdate < GETDATE())
	BEGIN
		DECLARE @WorkDate DATETIME
		SELECT @WorkDate = CONVERT(VARCHAR(10), @FromDate, 111)
		WHILE (DATEDIFF(d, @WorkDate, @ToDate) >= 0)
		BEGIN 
			INSERT INTO dbo.tmpEmployeeAttendace(EmpId, WorkDate, InsertedDate)
			SELECT EmpId, @WorkDate, GETDATE()
			FROM dbo.Employee AS e WITH (NOLOCK)
			WHERE [Enabled] = 1

			SET @WorkDate = DATEADD(d, 1, @WorkDate)
		END
	END
		
	UPDATE dbo.Holiday WITH(ROWLOCK,UPDLOCK)
	SET 
		HolidayName = @HolidayName,
		FromDate = @FromDate,
		ToDate = @ToDate 
	WHERE ID=@ID
end
COMMIT TRANSACTION;



