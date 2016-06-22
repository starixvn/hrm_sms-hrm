---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Group_Update]
	@GroupId int,
	@Code varchar(20),
    @Name nvarchar(50),
	@DepartmentId INT,
	@MailGroup VARCHAR(50) = '',  
    @LeaderId int,
    @AssistantLeaderId int = NULL, 
	@ModifiedDate DATETIME,
    @ModifiedBy int,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Update group 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
SET NOCOUNT ON

DECLARE @GroupId_other int

select TOP 1 @GroupId_other = GroupId  from dbo.[Group] with(nolock) where Code = @code AND IsDeleted = 0

if @GroupId <> @GroupId_other
begin
	SELECT @ErrCode = 100, @ErrMsg = N'Code "' + @code + '" have been used by other Group.'
	return
end

UPDATE [dbo].[Group]  WITH(RowLock,UPDLock)
SET Code = @Code
		,[Name] = @Name
		,DepartmentId = @DepartmentId
		,[MailGroup] = @MailGroup		  
		,LeaderId = @LeaderId
		,AssistantLeaderId = @AssistantLeaderId
		,ModifiedDate = @ModifiedDate
		,ModifiedBy = @ModifiedBy
WHERE GroupId=@GroupId
SELECT @ErrCode = 0, @ErrMsg = ''
END


