CREATE TABLE [dbo].[Message] (
    [MessageId]    INT            IDENTITY (1, 1) NOT NULL,
    [Subject]      NVARCHAR (100) NOT NULL,
    [Contents]     NVARCHAR (MAX) NOT NULL,
    [Category]     TINYINT        NOT NULL,
    [FromDate]     DATETIME       NULL,
    [ToDate]       DATETIME       NULL,
    [IsDisabled]   BIT            NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_MessageId] PRIMARY KEY CLUSTERED ([MessageId] ASC)
);

