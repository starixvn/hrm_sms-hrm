CREATE TABLE [dbo].[HRM_Section] (
    [SiteId]       SMALLINT       NOT NULL,
    [SectionId]    INT            NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NULL,
    [DepartmentId] INT            NULL,
    [IsActive]     BIT            NULL,
    [IsDeleted]    BIT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_HRM_Section] PRIMARY KEY CLUSTERED ([SiteId] ASC, [SectionId] ASC)
);

