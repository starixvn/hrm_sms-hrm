CREATE TABLE [dbo].[LookupData] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [LookupId] INT            NOT NULL,
    [Code]     VARCHAR (20)   NULL,
    [Name]     NVARCHAR (100) NULL,
    [Value]    VARCHAR (500)  NULL,
    CONSTRAINT [PK_LookupData] PRIMARY KEY CLUSTERED ([Id] ASC) ON [EmpCommon]
);

