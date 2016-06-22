CREATE TABLE [dbo].[OTMonthlyRequest] (
    [Month]          DATETIME        NOT NULL,
    [RequestId]      BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]          INT             NOT NULL,
    [NormalHours]    DECIMAL (18, 2) NULL,
    [OffHours]       DECIMAL (18, 2) NULL,
    [PublicHours]    DECIMAL (18, 2) NULL,
    [CurrentStatus]  TINYINT         NULL,
    [VerifierId]     INT             NULL,
    [ApproverId]     INT             NULL,
    [HRId]           INT             NULL,
    [VerifiedDate]   DATETIME        NULL,
    [ApprovedDate]   DATETIME        NULL,
    [HRDate]         DATETIME        NULL,
    [VerifiedStatus] TINYINT         NULL,
    [ApprovedStatus] TINYINT         NULL,
    [HRStatus]       TINYINT         NULL,
    [ModifiedBy]     INT             NULL,
    [ModifiedDate]   DATETIME        NULL,
    [CreatedDate]    DATETIME        NULL,
    [CreatedBy]      INT             NULL,
    CONSTRAINT [PK_OTMonthlyRequest1] PRIMARY KEY CLUSTERED ([Month] ASC, [EmpId] ASC) ON [EmpRequest]
);

