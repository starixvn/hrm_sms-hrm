CREATE TABLE [dbo].[ChangeShiftRequest] (
    [RequestId] BIGINT   NOT NULL,
    [EmpId]     INT      NOT NULL,
    [FromDate]  DATETIME NOT NULL,
    [ToDate]    DATETIME NOT NULL,
    [TypeId] TINYINT NOT NULL, 
    CONSTRAINT [PK_ChangeShiftRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
);

