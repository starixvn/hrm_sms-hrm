CREATE TABLE [dbo].[Function] (
    [FunctionId]       INT            IDENTITY (1, 1) NOT NULL,
    [FunctionKey]      VARCHAR (50)   NOT NULL,
    [Name]             NVARCHAR (200) NOT NULL,
    [ParentFunctionId] INT            NULL,
    [IsActive]         BIT            NULL,
    [Policy]       TINYINT            NULL,
    CONSTRAINT [PK_FunctionKey] PRIMARY KEY CLUSTERED ([FunctionId] ASC)
);

