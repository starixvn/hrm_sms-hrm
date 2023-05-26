CREATE TABLE [dbo].[HRM_Team] (
    [SiteId]       SMALLINT       NOT NULL,
    [TeamId]       INT            NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NULL,
    [SectionId]    INT            NULL,
    [IsDeleted]    BIT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_HRM_Team] PRIMARY KEY CLUSTERED ([SiteId] ASC, [TeamId] ASC)
);

