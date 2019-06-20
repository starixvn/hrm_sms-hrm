
CREATE PROCEDURE [dbo].[WorkingTimeRequest_Delete] @RequestId BIGINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-04
Task: delete a request form
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RequestType INT
		,@CurrentStatus INT
		,@KIVLeave DECIMAL(18,2)
		,@EmpId INT

	SELECT TOP 1 @RequestType = RequestType
		,@CurrentStatus = CurrentStatus
		,@EmpId = EmpId
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestID = @RequestID

	IF (@CurrentStatus <> 1)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'This Status Request have been changed. You cannot delete. '

		RETURN;
	END

	IF @RequestType IN (1, 2)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION

			DELETE WorkingTimeRequest
			WHERE RequestID = @RequestID

			DELETE InoutMissRequest
			WHERE RequestID = @RequestID

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION;

			SELECT @ErrCode = 101
				,@ErrMsg = N'Delete request failed.'
		END CATCH;
	END
	ELSE IF @RequestType IN (3, 4)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION

			DELETE WorkingTimeRequest
			WHERE RequestID = @RequestID

			DELETE [InOutRequest]
			WHERE RequestID = @RequestID

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION;

			SELECT @ErrCode = 101
				,@ErrMsg = N'Delete request failed.'
		END CATCH;
	END
	ELSE IF @RequestType IN (5)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION

			DELETE WorkingTimeRequest
			WHERE RequestID = @RequestID

			DELETE ChangeShiftRequest
			WHERE RequestID = @RequestID

			DELETE ChangeShiftRequestDetail
			WHERE RequestID = @RequestID

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION;

			SELECT @ErrCode = 101
				,@ErrMsg = N'Delete request failed.'
		END CATCH;
	END
	ELSE IF @RequestType IN (6)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION

			DELETE WorkingTimeRequest
			WHERE RequestID = @RequestID

			DELETE LeaveRequest
			WHERE RequestID = @RequestID

			DELETE LeaveRequestDetail
			WHERE RequestID = @RequestID

			DELETE
			FROM dbo.tmpLeaveBalance
			WHERE RequestID = @RequestID

			SELECT @KIVLeave = TotalDays
			FROM dbo.[tmpKIVLeaveBalance]
			WHERE RequestID = @RequestID

			IF(ISNULL(@KIVLeave, 0) > 0)
			BEGIN
				DELETE
				FROM dbo.[tmpKIVLeaveBalance]
				WHERE RequestID = @RequestID
			END

			DELETE
			FROM dbo.WeekendLeaveTrans
			WHERE RequestID = @RequestID

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION;

			SELECT @ErrCode = 101
				,@ErrMsg = N'Delete request failed.'
		END CATCH;
	END
	ELSE IF @RequestType IN (7)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION

			DELETE WorkingTimeRequest
			WHERE RequestID = @RequestID

			DELETE KeepInViewRequest
			WHERE RequestID = @RequestID

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION;

			SELECT @ErrCode = 101
				,@ErrMsg = N'Delete request failed.'
		END CATCH;
	END

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
