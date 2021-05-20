CREATE TABLE [dbo].[OTDailyRequest] (
    [RequestId]    BIGINT          NOT NULL,
    [OvertimeDate] DATE            NOT NULL,
    [EmpId]        INT             NOT NULL,
    [Schedule]     VARCHAR (20)    NULL,
    [TimeIn]       DATETIME        NULL,
    [TimeOut]      DATETIME        NULL,
    [TotalHours]   DECIMAL (18, 2) NULL,
    [NormalHours]  DECIMAL (18, 2) NULL,
    [OffHours]     DECIMAL (18, 2) NULL,
    [PublicHours]  DECIMAL (18, 2) NULL,
    [SpecialHours] DECIMAL (3, 1)  NULL,
    [Remark]       NVARCHAR (2000) NULL,
    [Status]       TINYINT         CONSTRAINT [DF_OTDailyRequest_Status] DEFAULT ((1)) NULL,
    [CreatedBy]    INT             NULL,
    [CreatedDate]  DATETIME        NULL,
    [IsUpdated]    BIT             CONSTRAINT [DF_OTDailyRequest_IsUpdated] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_OvertimeDailyRequest] PRIMARY KEY CLUSTERED ([OvertimeDate] ASC, [EmpId] ASC) ON [EmpRequest]
) ON [EmpRequest];



