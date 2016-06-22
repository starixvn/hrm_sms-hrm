CREATE TABLE [dbo].[tmpLogAttendance] (
    [Name]         VARCHAR (50) NULL,
    [EmpId]        INT          NULL,
    [FromDate]     DATETIME     NULL,
    [ToDate]       DATETIME     NULL,
    [IsOnlyUpdate] BIT          NULL,
    [InsertDate]   DATETIME     NULL
);

