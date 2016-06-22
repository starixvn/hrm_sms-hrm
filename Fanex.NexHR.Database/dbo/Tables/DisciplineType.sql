CREATE TABLE [dbo].[DisciplineType] (
    [DisciplineTypeId] INT            NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Score]            DECIMAL (5, 2) NOT NULL,
    [IsActive]         BIT            NOT NULL,
    CONSTRAINT [PK_DisciplineType] PRIMARY KEY CLUSTERED ([DisciplineTypeId] ASC)
);

