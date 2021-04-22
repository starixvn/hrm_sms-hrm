CREATE TABLE [dbo].[JobLevel] (
    [JobLevelId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (150) NULL,
    [IsActive]    BIT            NULL,
    [CreatedBy]   INT            NULL,
    [CreatedDate] DATETIME       NULL,
    CONSTRAINT [PK_JobLevel] PRIMARY KEY CLUSTERED ([JobLevelId] ASC)
);

