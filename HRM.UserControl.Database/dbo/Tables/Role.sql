CREATE TABLE [dbo].[Role] (
    [Id]           SMALLINT      IDENTITY (1, 1) NOT NULL,
    [SiteId]       SMALLINT      NOT NULL,
    [RoleId]       SMALLINT      NOT NULL,
    [Code]         VARCHAR (50)  NULL,
    [Name]         NVARCHAR (50) NULL,
    [IsActive]     BIT           CONSTRAINT [DF_Role_IsActive] DEFAULT ((1)) NULL,
    [IsDefault]    BIT           CONSTRAINT [DF_Role_IsDefault] DEFAULT ((0)) NULL,
    [CreatedBy]    INT           NULL,
    [CreatedDate]  DATETIME      NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([SiteId] ASC, [RoleId] ASC)
);



