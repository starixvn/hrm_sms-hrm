CREATE TABLE [dbo].[ChangeShiftRequest] (
    [RequestId] BIGINT   NOT NULL,
    [EmpId]     INT      NULL,
    [FromDate]  DATETIME NULL,
    [ToDate]    DATETIME NULL,
    CONSTRAINT [PK_ChangeShiftRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
);

