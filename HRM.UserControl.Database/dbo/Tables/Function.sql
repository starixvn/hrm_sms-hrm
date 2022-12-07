CREATE TABLE [dbo].[Function] (
    [FunctionId]   INT            IDENTITY (1, 1) NOT NULL,
    [SiteId]       SMALLINT       NOT NULL,
    [FunctionKey]  VARCHAR (100)  NOT NULL,
    [FunctionName] NVARCHAR (100) NOT NULL,
    [Description]  NVARCHAR (200) NULL,
    [ParentId]     INT            NULL,
    [Policy]       TINYINT        NULL,
    [IsActive]     BIT            CONSTRAINT [DF_Function_IsActive] DEFAULT ((1)) NOT NULL,
    [DisplayOrder] SMALLINT       NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_Function] PRIMARY KEY CLUSTERED ([FunctionId] ASC)
);

