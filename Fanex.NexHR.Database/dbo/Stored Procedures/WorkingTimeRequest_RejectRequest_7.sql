
CREATE PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest_7] @RequestId BIGINT
	,@EmpId INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@RoleID INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-07
Task: disapprove Keep In View request form (type 7)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@IsHalfDay BIT

	SELECT @FromDate = lr.FromDate
		,@ToDate = lr.ToDate
		,@IsHalfDay = IsHalfDay
	FROM dbo.KeepInViewRequest lr WITH (NOLOCK)
	WHERE lr.RequestID = @RequestID

	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 3
				,VerifiedStatus = 3
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 5
				,ApprovedStatus = 3
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 7
				,HRStatus = 3
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId

			UPDATE EmployeeSummary WITH (ROWLOCK, UPDLOCK)
			SET KeepInViewBalance = ISNULL(KeepInViewBalance, 0) - CASE 
					WHEN @IsHalfDay = 1 THEN 0.5
					ELSE 1
					END
			WHERE EmpId = @EmpId

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

		SET @ErrCode = 0
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END
