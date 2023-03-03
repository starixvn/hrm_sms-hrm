CREATE TABLE [dbo].[UnitHeadSchedule] (
    [SiteId]        SMALLINT       NOT NULL,
    [EmpId]         INT            NOT NULL,
    [WorkDate]      DATE           NOT NULL,
    [IsWorkDay]     BIT            NULL,
    [IsOffDay]      BIT            NULL,
    [IsHoliday]     BIT            NULL,
    [IsLeaveDay]    BIT            NULL,
    [LeaveTypeCode] NVARCHAR (50)  NULL,
    [LeaveTypeName] NVARCHAR (200) NULL,
    [SyncDate]      DATETIME       NULL,
    CONSTRAINT [PK_UnitHeadSchedule] PRIMARY KEY CLUSTERED ([SiteId] ASC, [EmpId] ASC, [WorkDate] ASC)
);

