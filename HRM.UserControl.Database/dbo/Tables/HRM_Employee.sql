CREATE TABLE [dbo].[HRM_Employee] (
    [SiteId]        SMALLINT       NOT NULL,
    [EmpId]         INT            NOT NULL,
    [UserId]        INT            NULL,
    [Username]      NVARCHAR (50)  NULL,
    [Code]          NVARCHAR (20)  NULL,
    [Nickname]      NVARCHAR (50)  NULL,
    [FullName]      NVARCHAR (100) NULL,
    [TeamId]        INT            NULL,
    [JobTitleId]    SMALLINT       NULL,
    [JobLevelId]    SMALLINT       NULL,
    [Email]         NVARCHAR (100) NULL,
    [IsLocal]       BIT            NULL,
    [IsActive]      BIT            NULL,
    [IsDeleted]     BIT            NULL,
    [JoinedDate]    DATE           NULL,
    [ConfirmedDate] DATE           NULL,
    [ResignedDate]  DATE           NULL,
    [ModifiedDate]  DATETIME       NULL,
    [SyncDate]      DATETIME       NULL,
    CONSTRAINT [PK_HRM_Employee] PRIMARY KEY CLUSTERED ([SiteId] ASC, [EmpId] ASC)
);



