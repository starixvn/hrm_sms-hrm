CREATE TABLE [dbo].[KeepInViewRequest] (
    [RequestId]      BIGINT         NOT NULL,
    [EmpId]          INT            NULL,
    [IsHalfDay]      BIT            CONSTRAINT [DF_KeepInViewRequest_IsHalfDay] DEFAULT ((0)) NULL,
    [FromTime]       DATETIME       NULL,
    [ToTime]         DATETIME       NULL,
    [RefNo]          NVARCHAR (200) NULL,
    [OnDate]         DATE           NULL,
    [ShiftId]        INT            NULL,
    [KeepInViewType] TINYINT        NULL,
    CONSTRAINT [PK_KeepInViewRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
) ON [EmpRequest];








GO

 


