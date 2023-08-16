CREATE TABLE [dbo].[Manpower_AnnualRequestForm] (
    [RequestId]    INT           IDENTITY (1, 1) NOT NULL,
    [PeriodId]     SMALLINT      NOT NULL,
    [UnitName]     NVARCHAR (50) NULL,
    [StatusId]     TINYINT       NOT NULL,
    [VerifiedBy]   INT           NULL,
    [ApprovedBy]   INT           NULL,
    [CreatedBy]    INT           NULL,
    [CreatedDate]  DATETIME      NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL,
    CONSTRAINT [PK_Manpower_AnnualRequestForm] PRIMARY KEY CLUSTERED ([RequestId] ASC)
);

