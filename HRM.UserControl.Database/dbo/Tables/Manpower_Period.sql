CREATE TABLE [dbo].[Manpower_Period] (
    [PeriodId]     SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [Year]         SMALLINT       NOT NULL,
    [IsActive]     BIT            CONSTRAINT [Manpower_Period_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_Manpower_Period] PRIMARY KEY CLUSTERED ([PeriodId] ASC)
);

