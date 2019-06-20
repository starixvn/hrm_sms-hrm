
CREATE PROCEDURE [dbo].[Holiday_Update]
	@Id int,
	@HolidayName nvarchar(50),	
	@FromDate datetime,
	@ToDate datetime,
	@ModifiedBy int,
	@ModifiedDate datetime,
	@ErrCode INT = 0 OUTPUT,
	@ErrMsg VARCHAR(200) = '' OUTPUT 	  	
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
	WHERE ID=@Id
end
else
begin
	UPDATE dbo.EmployeeCalendar WITH(ROWLOCK,UPDLOCK)
	Set IsHoliday = 0
	WHERE WorkDate BETWEEN @OldFromdate AND @Oldtodate

	UPDATE cl
	SET cl.IsHoliday = 1
		,cl.ModifiedBy = @ModifiedBy
		,cl.ModifiedDate = @ModifiedDate
	FROM dbo.EmployeeCalendar AS cl WITH (ROWLOCK, UPDLOCK)
	WHERE WorkDate BETWEEN @FromDate
			AND @ToDate
		AND EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary WITH (NOLOCK)
			WHERE EmpId = cl.EmpId
				AND (
					IsLocal = 1
					OR (
						IsLocal = 0
						AND IsExpatUseHoliday = 1
						)
					)
			)

	DECLARE @WorkDate DATETIME
	SELECT @WorkDate = CONVERT(VARCHAR(10), @FromDate, 111)
	WHILE (DATEDIFF(d, @WorkDate, @ToDate) >= 0)
	BEGIN
		IF(@WorkDate  < GETDATE())
		BEGIN
			INSERT INTO dbo.tmpEmployeeAttendace(EmpId, WorkDate, InsertedDate)
			SELECT EmpId, @WorkDate, GETDATE()
			FROM dbo.Employee AS e WITH (NOLOCK)
			WHERE [Enabled] = 1
		END

		INSERT INTO dbo.EmployeeCalendar(EmpId, WorkDate, IsHoliday, ModifiedDate)
		SELECT EmpId, @WorkDate, 1, GETDATE()
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE [Enabled] = 1
		AND EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary WITH (NOLOCK)
			WHERE EmpId = e.EmpId
				AND (
					IsLocal = 1
					OR (
						IsLocal = 0
						AND IsExpatUseHoliday = 1
						)
					)
			)
		AND NOT EXISTS (SELECT 1 FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE EmpId = e.EmpId AND WorkDate = @WorkDate
		)

		SET @WorkDate = DATEADD(d, 1, @WorkDate)
	END
		
	UPDATE dbo.Holiday WITH(ROWLOCK,UPDLOCK)
	SET 
		HolidayName = @HolidayName,
		FromDate = @FromDate,
		ToDate = @ToDate 
	WHERE ID=@ID

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1, @ErrMsg = N'Update Holiday failed.'
	END;
	ELSE
	BEGIN
		SELECT @ErrCode = 0, @ErrMsg = ''
	END;
end
COMMIT TRANSACTION;



