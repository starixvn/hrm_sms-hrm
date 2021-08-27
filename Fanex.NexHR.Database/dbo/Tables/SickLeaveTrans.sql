CREATE TABLE [dbo].[SickLeaveTrans] (
    [TransId]      BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT             NOT NULL,
    [RequestId]    BIGINT          NULL,
    [Description]  NVARCHAR (4000) NULL,
    [FromDate]     DATETIME        NOT NULL,
    [ToDate]       DATETIME        NULL,
    [DeductDays]   NUMERIC (18, 1) NULL,
    [CreditDays]   NUMERIC (18, 1) NULL,
    [IsDeleted]    BIT             CONSTRAINT [DF_SickLeaveTrans_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedDate]  DATETIME        NULL,
    [CreatedBy]    INT             NULL,
    [ModifiedDate] DATETIME        NULL,
    [ModifiedBy]   INT             NULL,
    CONSTRAINT [PK_SickLeaveTrans] PRIMARY KEY CLUSTERED ([TransId] ASC) ON [EmpRequest]
) ON [EmpRequest];

