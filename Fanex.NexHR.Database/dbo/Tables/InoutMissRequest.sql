CREATE TABLE [dbo].[InoutMissRequest] (
    [RequestId]   BIGINT         NOT NULL,
    [EmpId]       INT            NOT NULL,
    [OnDate]      DATETIME       NOT NULL,
    [CheckTime]   DATETIME       NOT NULL,
    [RefNo]       NVARCHAR (200) NULL,
    [ReworkCount] INT            CONSTRAINT [DF_InoutMissRequest_ReworkCount] DEFAULT ((0)) NULL,
    [ReasonId]    TINYINT        NULL,
    CONSTRAINT [PK_InoutMissRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
) ON [EmpRequest];




GO

 


