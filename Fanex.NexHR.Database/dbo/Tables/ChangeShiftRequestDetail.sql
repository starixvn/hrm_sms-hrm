CREATE TABLE [dbo].[ChangeShiftRequestDetail] (
    [RequestId] BIGINT          NOT NULL,
    [EmpId]     INT             NOT NULL,
    [OnDate]    DATE            NOT NULL,
    [FromShift] INT             NULL,
    [ToShift]   INT             NULL,
    [Remark]    NVARCHAR (2000) NULL,
    [ToDate]    DATE            NULL,
    CONSTRAINT [PK_ChangeShiftRequestDetail] PRIMARY KEY CLUSTERED ([OnDate] ASC, [EmpId] ASC, [RequestId] ASC) ON [EmpRequest]
) ON [EmpRequest];



