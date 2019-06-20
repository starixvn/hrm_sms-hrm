
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_VoidRequest_bySA_7    Script Date: 7/20/2016 10:19:26 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_VoidRequest_bySA_7    Script Date: 7/19/2016 5:22:24 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_VoidRequest_bySA_7] @RequestID BIGINT
	,@ProcessedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151026@Quinn.Vo
Task: Void request late in by SA
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @CurrentStatus INT
		,@ProcessedDate DATETIME
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@EmpId INT
		,@IsHalfDay BIT

	SELECT @CurrentStatus = wk.[CurrentStatus]
		,@ProcessedDate = GETDATE()
		,@FromDate = rq.FromDate
		,@ToDate = ToDate
		,@EmpId = wk.EmpId
		,@IsHalfDay = IsHalfDay
	FROM dbo.WorkingTimeRequest wk WITH (NOLOCK)
		INNER JOIN dbo.KeepInViewRequest rq WITH (NOLOCK) ON wk.RequestId = rq.RequestId
	WHERE wk.RequestID = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 7
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

		IF @CurrentStatus IN (4, 6)
		BEGIN
			UPDATE cl WITH (ROWLOCK, UPDLOCK)
			SET cl.IsKeepInView = 0
				,cl.LeaveDay = 0
				,cl.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar cl
				INNER JOIN dbo.KeepInViewRequest csd WITH (NOLOCK) ON (
					cl.WorkDate BETWEEN csd.FROMDate
						AND csd.ToDate
					)
				AND cl.EmpId = csd.EmpId
			WHERE cl.EmpID = @EmpId
				AND csd.RequestID = @RequestID

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void request failed'

		ROLLBACK TRANSACTION
	END CATCH;
END