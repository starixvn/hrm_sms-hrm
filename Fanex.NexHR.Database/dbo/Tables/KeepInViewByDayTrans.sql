CREATE TABLE [dbo].[KeepInViewByDayTrans] (
    [EmpId]        INT             NOT NULL,
    [RequestId]    BIGINT          NOT NULL,
    [CreditDays]   DECIMAL (18, 2) NULL,
    [DeductDays]   DECIMAL (18, 2) NULL,
    [InYear]       INT             NULL,
    [InsertedDate] DATETIME        NULL
) ON [EmpRequest];

