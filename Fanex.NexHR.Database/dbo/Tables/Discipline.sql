CREATE TABLE [dbo].[Discipline] (
    [EmpId]                INT            NOT NULL,
    [WorkDate]             DATE           NOT NULL,
    [DisciplineScore]      DECIMAL (5, 2) NULL,
    [IsOutOfControlReason] BIT            NULL,
    [EmployeeComment]      NVARCHAR (MAX) NULL,
    [ApproverComment]      NVARCHAR (MAX) NULL,
    [IsMissingCheckInOut]  BIT            NULL,
    [IsLateIn]             BIT            NULL,
    [IsEarlyOut]           BIT            NULL,
    [IsWorkingUnderTime]   BIT            NULL,
    [IsChangeShift]        BIT            NULL,
    [IsAbsent]             BIT            NULL,
    [IsUnpaidLeave]        BIT            NULL,
    [IsUnpaidLeaveAbsent]  BIT            NULL,
    [IsUnplannedLeave]     BIT            NULL,
    [IsWeekendLeave]       BIT            NULL,
    [IsDeleted]            BIT            NULL,
    [ModifiedBy]           INT            NULL,
    [ModifiedDate]         DATETIME       NULL,
    CONSTRAINT [PK_Discipline] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);

