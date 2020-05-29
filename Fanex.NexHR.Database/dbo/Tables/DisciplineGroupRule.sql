CREATE TABLE [dbo].[DisciplineGroupRule] (
    [GroupId]          INT NOT NULL,
    [DisciplineTypeId] INT NOT NULL,
    [IsLocal]          BIT NOT NULL,
    [Value]            BIT NOT NULL,
    CONSTRAINT [PK_DisciplineGroupRule] PRIMARY KEY CLUSTERED ([GroupId] ASC, [DisciplineTypeId] ASC, [IsLocal] ASC)
);

