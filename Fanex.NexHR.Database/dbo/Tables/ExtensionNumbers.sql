CREATE TABLE [dbo].[ExtensionNumbers] (
    [ExtID]       INT            IDENTITY (1, 1) NOT NULL,
    [FullName]    NVARCHAR (200) NULL,
    [NickName]    NVARCHAR (100) NULL,
    [ExtensionNo] NVARCHAR (200) NULL,
    [IsInternal]  BIT            CONSTRAINT [DF_ExtensionNumbers_IsMembers] DEFAULT ((0)) NULL,
    [IsGroup]     BIT            CONSTRAINT [DF_ExtensionNumbers_IsGroup] DEFAULT ((0)) NULL,
    [EmpID]       INT            NULL,
    [GroupID]     INT            NULL,
    [OrderNo]     SMALLINT       NULL,
    CONSTRAINT [PK_ExtensionNumbers] PRIMARY KEY CLUSTERED ([ExtID] ASC) ON [EmpCommon]
);

