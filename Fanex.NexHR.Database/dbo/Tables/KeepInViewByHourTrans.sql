CREATE TABLE [dbo].[KeepInViewByHourTrans] (
    [EmpId]         INT      NOT NULL,
    [WorkDate]      DATE     NOT NULL,
    [RequestId]     BIGINT   NOT NULL,
    [CreditMinutes] INT      NULL,
    [DeductMinutes] INT      NULL,
    [InsertedDate]  DATETIME NULL,
    [MinutesBalance] INT      NULL,
    CONSTRAINT [PK_KeepInViewTrans] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC, [RequestId] ASC)
);

