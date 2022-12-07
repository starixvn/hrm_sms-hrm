CREATE TABLE [dbo].[Site] (
    [SiteId]       SMALLINT       IDENTITY (1, 1) NOT NULL,
    [CompanyId]    SMALLINT       NOT NULL,
    [SiteName]     NVARCHAR (50)  NOT NULL,
    [IsActive]     BIT            CONSTRAINT [DF_Site_IsActive] DEFAULT ((0)) NOT NULL,
    [SiteURL]      NVARCHAR (100) NULL,
    [AccessToken]  VARCHAR (100)  NOT NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([SiteId] ASC)
);

