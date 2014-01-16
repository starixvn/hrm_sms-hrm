ALTER PROCEDURE WorkingTimeRequest_ApproveRequest_3
	@RequestID bigint,	
	@EmpId int,
	@ApprovedBy int,
	@ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve late in request form (type 3)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get request latein dates
DECLARE @WorkDate datetime, @LateIn int

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
	SELECT @WorkDate = FromDate, @LateIn = LateInMins 
	FROM dbo.WorkingTimeRequest
	WHERE Id = @RequestID

	-- update the working calendar to record the check-in time
	UPDATE dbo.EmployeeCalendar
	SET LateIn = @LateIn,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy
	WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0

	-- process the calendar into attendance
	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 2 ,-- approved
		ProcessedBy= @ApprovedBy,
		ProcessedDate= @ApprovedDate,
		ModifiedDate = @ApprovedDate,
		ModifiedBy = @ApprovedBy
	WHERE Id = @RequestID
	 
	COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
	
	DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

	ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;	