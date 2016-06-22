CREATE TABLE [dbo].[Memo] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [RefNo]      NVARCHAR (50)   NOT NULL,
    [Contents]   NVARCHAR (MAX)  NOT NULL,
    [Subject]    NVARCHAR (4000) NOT NULL,
    [SentTo]     NVARCHAR (1000) NULL,
    [SentFrom]   NVARCHAR (1000) NULL,
    [ForInfo]    NVARCHAR (1000) NULL,
    [TotalPages] INT             NULL,
    [SentDate]   DATETIME        NULL,
    [Category]   INT             NULL,
    [Type]       INT             NULL,
    [Enabled]    BIT             NULL,
    CONSTRAINT [PK_Memo] PRIMARY KEY CLUSTERED ([Id] ASC) ON [EmpCommon]
) TEXTIMAGE_ON [EmpCommon];

