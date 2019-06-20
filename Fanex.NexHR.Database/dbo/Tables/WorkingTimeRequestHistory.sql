CREATE TABLE [dbo].[WorkingTimeRequestHistory] (
    [RequestId]       BIGINT          NOT NULL,
    [EmpId]           INT             NOT NULL,
    [RequestType]     INT             NOT NULL,
    [Remark]          NVARCHAR (2000) NULL,
    [CurrentStatus]   TINYINT         NULL,
    [VerifierId]      INT             NULL,
    [ApproverId]      INT             NULL,
    [HRId]            INT             NULL,
    [VerifiedDate]    DATETIME        NULL,
    [ApprovedDate]    DATETIME        NULL,
    [HRDate]          DATETIME        NULL,
    [VerifiedStatus]  TINYINT         NULL,
    [ApprovedStatus]  TINYINT         NULL,
    [HRStatus]        TINYINT         NULL,
    [InsertedDate]    DATETIME        NULL,
    [ModifiedDate]    DATETIME        NULL,
    [ModifiedBy]      INT             NULL,
    [CreatedBy]       INT             NULL,
    [CreatedDate]     DATETIME        NULL,
    [ReworkCount]     INT             CONSTRAINT [DF_WorkingTimeRequestHistory_ReworkCount] DEFAULT ((0)) NULL,
    [ApproverComment] NVARCHAR (2000) NULL,
    [HistoryId]       BIGINT          IDENTITY (1, 1) NOT NULL,
    [LogDate]         DATETIME        NULL
) ON [EmpLogging];



