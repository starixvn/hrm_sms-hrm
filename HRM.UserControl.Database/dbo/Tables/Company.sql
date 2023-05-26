CREATE TABLE [dbo].[Company] (
    [CompanyId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [Country]   NVARCHAR (50)  NULL,
    [IsActive]  BIT            CONSTRAINT [DF_Company_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_COMPANY] PRIMARY KEY CLUSTERED ([CompanyId] ASC),
    CONSTRAINT [UQ__Company__737584F67C9C7487] UNIQUE NONCLUSTERED ([Name] ASC)
);



