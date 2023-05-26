CREATE TABLE [dbo].[HRM_JobTitle] (
    [SiteId]     SMALLINT       NOT NULL,
    [JobTitleId] INT            NOT NULL,
    [Name]       NVARCHAR (200) NULL,
    [IsDeleted]  BIT            NULL,
    [SyncDate]   DATETIME       NULL,
    CONSTRAINT [PK_HRM_JobTitle] PRIMARY KEY CLUSTERED ([SiteId] ASC, [JobTitleId] ASC)
);

