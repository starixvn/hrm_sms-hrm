CREATE TABLE [dbo].[OtherDiscipline] (
    [EmpId]            INT            NOT NULL,
    [WorkDate]         DATE           NOT NULL,
    [DisciplineTypeId] SMALLINT       NOT NULL,
    [Score]            DECIMAL (5, 2) CONSTRAINT [DF_OtherDiscipline_Score] DEFAULT ((0)) NOT NULL,
    [Comment]          NVARCHAR (500) NULL,
    [IsDeleted]        BIT            CONSTRAINT [DF_OtherDiscipline_IsDeleted] DEFAULT ((0)) NOT NULL,
    [ModifiedBy]       INT            NULL,
    [ModifiedDate]     DATETIME       NULL,
    CONSTRAINT [PK_OtherDiscipline] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC, [DisciplineTypeId] ASC)
);


