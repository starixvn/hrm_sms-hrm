CREATE TABLE [dbo].[Lookup] (
    [Id]   INT          IDENTITY (1, 1) NOT NULL,
    [Code] VARCHAR (20) NULL,
    [Name] VARCHAR (50) NULL,
    CONSTRAINT [PK_Lookup] PRIMARY KEY CLUSTERED ([Id] ASC) ON [EmpCommon]
);

