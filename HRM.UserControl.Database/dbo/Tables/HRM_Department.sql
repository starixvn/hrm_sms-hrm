CREATE TABLE [dbo].[HRM_Department] (
    [SiteId]       SMALLINT       NOT NULL,
    [DepartmentId] INT            NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NULL,
    [UnitId]       INT            NULL,
    [IsActive]     BIT            NULL,
    [IsDeleted]    BIT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_HRM_Department] PRIMARY KEY CLUSTERED ([SiteId] ASC, [DepartmentId] ASC)
);

