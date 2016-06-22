CREATE TABLE [dbo].[WorkingTimeRequest] (
    [RequestId]       BIGINT          IDENTITY (1, 1) NOT NULL,
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
    [CreatedBy]       INT             NULL,
    [CreatedDate]     DATETIME        NULL,
    [ModifiedBy]      INT             NULL,
    [ModifiedDate]    DATETIME        NULL,
    [ReworkCount]     INT             CONSTRAINT [DF_WorkingTimeRequest_ReworkCount] DEFAULT ((0)) NULL,
    [ApproverComment] NVARCHAR (2000) NULL,
    CONSTRAINT [PK_WorkingTimeRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
);

