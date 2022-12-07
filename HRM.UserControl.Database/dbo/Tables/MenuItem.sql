CREATE TABLE [dbo].[MenuItem] (
    [MenuItemId]   INT            IDENTITY (1, 1) NOT NULL,
    [FunctionId]   INT            NULL,
    [SiteId]       SMALLINT       NOT NULL,
    [ParentId]     INT            NULL,
    [MenuItemName] VARCHAR (100)  NULL,
    [Description]  NVARCHAR (200) NULL,
    [URL]          VARCHAR (100)  NULL,
    [IsActive]     BIT            CONSTRAINT [DF_MenuItem_IsActive] DEFAULT ((1)) NULL,
    [DisplayOrder] SMALLINT       NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_MenuItem] PRIMARY KEY CLUSTERED ([MenuItemId] ASC)
);

