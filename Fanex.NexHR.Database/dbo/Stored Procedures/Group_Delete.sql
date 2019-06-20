/****** Object:  Stored Procedure dbo.Group_Delete    Script Date: 7/20/2016 10:19:09 AM ******/
/****** Object:  Stored Procedure dbo.Group_Delete    Script Date: 7/19/2016 5:22:15 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Group_Delete]
	@GroupID int,
    @ModifiedBy int,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Delete Group 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN

SET NOCOUNT ON;

IF (EXISTS(SELECT TOP 1 1 FROM dbo.[Group]  eg WITH (NOLOCK) 
						INNER JOIN Employee e WITH (NOLOCK) ON eg.GroupID=e.GroupID
		WHERE eg.GroupID=@GroupID AND e.isDeleted=0 AND e.[Enabled] = 1))
BEGIN
	SELECT @ErrCode=100,
			@ErrMsg = 'The group is not empty so cannot delete it'
	RETURN
END

UPDATE [dbo].[Group]   WITH(RowLock,UPDLock)
SET IsDeleted = 1
	,ModifiedDate = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE GroupID=@GroupID

SELECT @ErrCode = 0, @ErrMsg = ''

END


