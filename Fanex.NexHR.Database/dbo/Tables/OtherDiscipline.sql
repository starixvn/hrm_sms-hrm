CREATE TABLE [dbo].[OtherDiscipline]
(
	[EmpId] INT NOT NULL , 
    [WorkDate] DATE NOT NULL, 
    [DisciplineTypeId] SMALLINT NOT NULL, 
    [Score] DECIMAL(5, 2) NOT NULL DEFAULT 0, 
    [Comment] NVARCHAR(500) NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    [ModifiedBy] INT NULL, 
    [ModifiedDate] DATETIME NULL, 
    PRIMARY KEY ([EmpId], [WorkDate], [DisciplineTypeId])
)
