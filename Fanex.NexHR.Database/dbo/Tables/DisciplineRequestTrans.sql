CREATE TABLE [dbo].[DisciplineRequestTrans] (
    [EmpId]            INT      NOT NULL,
    [WorkDate]         DATE     NOT NULL,
    [RequestId]        BIGINT   NOT NULL,
    [DisciplineTypeId] INT      NOT NULL,
    [IsIgnored]        BIT      NOT NULL,
    [IsOutOfControl]   BIT      NOT NULL,
    [ModifiedDate]     DATETIME NULL,
    [ModifiedBy]       INT      NULL,
    CONSTRAINT [PK_DisciplineRequestTrans] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC, [RequestId] ASC, [DisciplineTypeId] ASC)
);

