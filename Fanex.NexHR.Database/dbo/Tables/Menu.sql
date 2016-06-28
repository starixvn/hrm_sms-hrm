CREATE TABLE [dbo].[Menu] (
    [MenuId]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [Text]         NVARCHAR (100) NULL,
    [Link]         NVARCHAR (100) NULL,
    [ParentId]     INT            NULL,
    [FunctionId]   INT            NULL,
    [DisplayOrder] INT            NULL,
    [IsActive]     BIT            NULL,
    [Policy]       INT            NULL,
    CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED ([MenuId] ASC)
);



