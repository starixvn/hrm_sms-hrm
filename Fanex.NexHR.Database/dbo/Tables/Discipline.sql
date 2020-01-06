CREATE TABLE [dbo].[Discipline] (
    [EmpId]                INT            NOT NULL,
    [WorkDate]             DATE           NOT NULL,
    [DisciplineScore]      DECIMAL (5, 2) CONSTRAINT [DF_Discipline_DisciplineScore] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlReason] BIT            CONSTRAINT [DF_Discipline_IsOutOfControlReason] DEFAULT ((0)) NOT NULL,
    [EmployeeComment]      NVARCHAR (MAX) NULL,
    [ApproverComment]      NVARCHAR (MAX) NULL,
    [IsMissingCheckInOut]  BIT            CONSTRAINT [DF_Discipline_IsMissingCheckInOut] DEFAULT ((0)) NOT NULL,
    [IsLateIn]             BIT            CONSTRAINT [DF_Discipline_IsLateIn] DEFAULT ((0)) NOT NULL,
    [IsEarlyOut]           BIT            CONSTRAINT [DF_Discipline_IsEarlyOut] DEFAULT ((0)) NOT NULL,
    [IsWorkingUnderTime]   BIT            CONSTRAINT [DF_Discipline_IsWorkingUnderTime] DEFAULT ((0)) NOT NULL,
    [IsChangeShift]        BIT            CONSTRAINT [DF_Discipline_IsChangeShift] DEFAULT ((0)) NOT NULL,
    [IsAbsent]             BIT            CONSTRAINT [DF_Discipline_IsAbsent] DEFAULT ((0)) NOT NULL,
    [IsUnpaidLeave]        BIT            CONSTRAINT [DF_Discipline_IsUnpaidLeave] DEFAULT ((0)) NOT NULL,
    [IsUnpaidLeaveAbsent]  BIT            CONSTRAINT [DF_Discipline_IsUnpaidLeaveAbsent] DEFAULT ((0)) NOT NULL,
    [IsUnplannedLeave]     BIT            CONSTRAINT [DF_Discipline_IsUnplannedLeave] DEFAULT ((0)) NOT NULL,
    [IsWeekendLeave]       BIT            CONSTRAINT [DF_Discipline_IsWeekendLeave] DEFAULT ((0)) NOT NULL,
    [IsDeleted]            BIT            CONSTRAINT [DF_Discipline_IsDeleted] DEFAULT ((0)) NOT NULL,
    [ModifiedBy]           INT            NULL,
    [ModifiedDate]         DATETIME       NULL,
    CONSTRAINT [PK_Discipline] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);



