CREATE TABLE [dbo].[ChangeShiftRequestDetail] (
    [OnDate]    DATETIME        NOT NULL,
    [EmpId]     INT             NOT NULL,
    [RequestId] BIGINT          NOT NULL,
    [FromShift] INT             NULL,
    [ToShift]   INT             NULL,
    [Remark]    NVARCHAR (2000) NULL,
    CONSTRAINT [PK_ChangeShiftRequestDetail] PRIMARY KEY CLUSTERED ([OnDate] ASC, [EmpId] ASC, [RequestId] ASC) ON [EmpRequest]
);

