/****** Object:  Stored Procedure dbo.Memo_Delete    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.Memo_Delete    Script Date: 7/19/2016 5:22:17 PM ******/

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Memo_Delete] @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: DELETE Memo
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	DELETE MEMO WITH (ROWLOCK)
	WHERE Id = @Id

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete Memo failed.';
	END;
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = '';
	END;
END
