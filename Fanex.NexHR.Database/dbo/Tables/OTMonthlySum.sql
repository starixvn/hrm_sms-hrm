CREATE TABLE [dbo].[OTMonthlySum] (
    [Month]       DATETIME        NOT NULL,
    [EmpId]       INT             NOT NULL,
    [NormalHours] DECIMAL (18, 2) NULL,
    [OffHours]    DECIMAL (18, 2) NULL,
    [PublicHours] DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_OTMonthlySum] PRIMARY KEY CLUSTERED ([Month] ASC, [EmpId] ASC) ON [EmpRequest]
);

