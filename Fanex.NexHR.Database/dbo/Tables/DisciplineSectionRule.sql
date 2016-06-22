CREATE TABLE [dbo].[DisciplineSectionRule] (
    [GroupId]          INT NOT NULL,
    [DisciplineTypeId] INT NOT NULL,
    [IsLocal]          BIT NOT NULL,
    [Value]            BIT NOT NULL,
    CONSTRAINT [PK_DisciplineSectionRule] PRIMARY KEY CLUSTERED ([GroupId] ASC, [DisciplineTypeId] ASC, [IsLocal] ASC)
);

