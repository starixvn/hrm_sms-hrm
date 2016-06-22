CREATE TABLE [dbo].[LogAction] (
    [LogActionId] INT             NOT NULL,
    [Name]        NVARCHAR (4000) NOT NULL,
    [SpName]      VARCHAR (500)   NULL,
    CONSTRAINT [PK_LogAction_1] PRIMARY KEY CLUSTERED ([LogActionId] ASC) ON [EmpLogging]
);

