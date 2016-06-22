CREATE TABLE [dbo].[SystemSetting] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Code]  VARCHAR (50)   NOT NULL,
    [Value] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_SystemSetting] PRIMARY KEY CLUSTERED ([Id] ASC) ON [EmpCommon]
);

