/****** Object:  Stored Procedure dbo.WorkingTimeRequest_VoidRequest_bySA_3    Script Date: 7/20/2016 10:19:26 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_VoidRequest_bySA_3    Script Date: 7/19/2016 5:22:24 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_VoidRequest_bySA_3] @RequestID BIGINT
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

	SELECT @CurrentStatus = wk.[CurrentStatus]
		,@ProcessedDate = GETDATE()
		,@FromDate = rq.FromDate
		,@ToDate = ToDate
		,@EmpId = wk.EmpId
	FROM dbo.WorkingTimeRequest wk WITH (NOLOCK)
		INNER JOIN dbo.InOutRequest rq WITH (NOLOCK) ON wk.RequestId = rq.RequestId
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
			UPDATE EC
			SET LateIn = 0
				,EC.ModifiedDate = @ProcessedDate
			FROM EmployeeCalendar EC WITH (ROWLOCK, UPDLOCK)
			WHERE WorkDate BETWEEN @FromDate
					AND @ToDate
				AND EC.EmpID = @EmpId

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