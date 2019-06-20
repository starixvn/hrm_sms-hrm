/****** Object:  Stored Procedure dbo.ExtensionNO_DELETE_EmployeeInfo    Script Date: 7/20/2016 10:19:08 AM ******/
/****** Object:  Stored Procedure dbo.ExtensionNO_DELETE_EmployeeInfo    Script Date: 7/19/2016 5:22:15 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[ExtensionNO_DELETE_EmployeeInfo]
	@ExtID int,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT	
AS
/*
Created: Tom.Vo@2014-01-04
Task: Delete Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON


IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK) WHERE ExtID = @ExtID AND [IsGroup]=0)
BEGIN
	SELECT @ErrCode = 100, @ErrMsg = N'Parameter @ExtID not exists on systems.'
	RETURN
END
ELSE
	DELETE FROM [dbo].[ExtensionNumbers]  WHERE ExtID = @ExtID

SELECT @ErrCode = 0,@ErrMsg=''


