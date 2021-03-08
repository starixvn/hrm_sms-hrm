CREATE TABLE [dbo].[RefResource] (
    [RefResourceId] INT            IDENTITY (1, 1) NOT NULL,
    [RefName]       VARCHAR (100)  NOT NULL,
    [ResourceId]    INT            NOT NULL,
    [Value]         NVARCHAR (500) NOT NULL,
    [Description]   NVARCHAR (500) NULL,
    [IsActive]      BIT            CONSTRAINT [DF_RefResource_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_RefResource] PRIMARY KEY CLUSTERED ([RefResourceId] ASC)
);

