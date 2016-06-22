CREATE TABLE [dbo].[ShiftArrangementOrder] (
    [OwnerId]      INT          NOT NULL,
    [EmpId]        INT          NOT NULL,
    [OrderNo]      SMALLINT     NULL,
    [OrderCode]    VARCHAR (10) NULL,
    [ModifiedDate] DATETIME     NULL,
    CONSTRAINT [PK_ShiftArrangementOrder] PRIMARY KEY CLUSTERED ([OwnerId] ASC, [EmpId] ASC)
);

