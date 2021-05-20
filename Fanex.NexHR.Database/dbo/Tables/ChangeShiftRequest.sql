CREATE TABLE [dbo].[ChangeShiftRequest] (
    [RequestId] BIGINT  NOT NULL,
    [EmpId]     INT     NOT NULL,
    [FromDate]  DATE    NOT NULL,
    [ToDate]    DATE    NOT NULL,
    [TypeId]    TINYINT CONSTRAINT [DF_ChangeShiftRequest_TypeId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ChangeShiftRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
) ON [EmpRequest];





