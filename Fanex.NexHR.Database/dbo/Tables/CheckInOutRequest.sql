CREATE TABLE [dbo].[CheckInOutRequest] (
    [RequestId]   BIGINT         NOT NULL,
    [EmpId]       INT            NOT NULL,
    [OnDate]      DATETIME       NOT NULL,
    [RequestTime] DATETIME       NOT NULL,
    [ReasonId]    TINYINT        NULL,
    [RefNo]       NVARCHAR (200) NULL,
    CONSTRAINT [PK_CheckInOutRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
) ON [EmpRequest];

