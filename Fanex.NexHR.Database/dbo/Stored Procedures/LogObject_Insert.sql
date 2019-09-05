CREATE PROCEDURE [dbo].[LogObject_Insert] @UserId INT
	,@ObjectTypeId TINYINT
	,@ObjectId INT
	,@OldData NVARCHAR(MAX) = NULL
	,@Data NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190904@Tim.Trinh
Task: insert log
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	INSERT INTO dbo.LogObject (
		LogDate
		,UserId
		,ObjectTypeId
		,ObjectId
		,OldData
		,Data
		)
	VALUES (
		GETDATE()
		,@UserId
		,@ObjectTypeId
		,@ObjectId
		,@OldData
		,@Data
		)

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = N'Insert log failed.'

		RETURN
	END
END
