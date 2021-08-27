CREATE TABLE [dbo].[HospitalizationLeaveTrans] (
    [TransId]      BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT             NOT NULL,
    [RequestId]    BIGINT          NULL,
    [Description]  NVARCHAR (4000) NULL,
    [FromDate]     DATETIME        NOT NULL,
    [ToDate]       DATETIME        NULL,
    [DeductDays]   NUMERIC (18, 1) CONSTRAINT [DF_HospitalizationLeaveTrans_DeductDays] DEFAULT ((0)) NULL,
    [CreditDays]   NUMERIC (18, 1) CONSTRAINT [DF_HospitalizationLeaveTrans_CreditDays] DEFAULT ((0)) NULL,
    [IsDeleted]    BIT             CONSTRAINT [DF_HospitalizationLeaveTrans_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedDate]  DATETIME        NULL,
    [CreatedBy]    INT             NULL,
    [ModifiedDate] DATETIME        NULL,
    [ModifiedBy]   INT             NULL,
    CONSTRAINT [PK_HospitalizationLeaveTrans] PRIMARY KEY CLUSTERED ([TransId] ASC) ON [EmpRequest]
) ON [EmpRequest];

