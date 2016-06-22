CREATE TABLE [dbo].[MailSettings] (
    [GroupId]            INT NOT NULL,
    [Action]             INT NOT NULL,
    [AllowSendUser]      BIT NULL,
    [AllowSendVerifier]  BIT NULL,
    [AllowSendApprover]  BIT NULL,
    [AllowSendHR]        BIT NULL,
    [AllowSendMailGroup] BIT NULL,
    CONSTRAINT [PK_MailSettings] PRIMARY KEY CLUSTERED ([GroupId] ASC, [Action] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1: New, 2: Verify, 3: Not Verify, 4: Approve, 5: Reject, 6: Process, 7: Void, 8: Reset1, 9: Reset2, 10: Reset3, 11: Edit, 12: Delete, 13: Reedit', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MailSettings', @level2type = N'COLUMN', @level2name = N'Action';

