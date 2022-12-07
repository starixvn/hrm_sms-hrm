CREATE TABLE [dbo].[User] (
    [UserId]             INT            IDENTITY (1, 1) NOT NULL,
    [Username]           VARCHAR (50)   NULL,
    [Code]               VARCHAR (20)   NULL,
    [Nickname]           NVARCHAR (50)  NULL,
    [FullName]           NVARCHAR (100) NULL,
    [Password]           VARCHAR (100)  NULL,
    [OldPassword]        VARCHAR (100)  NULL,
    [Email]              VARCHAR (50)   NULL,
    [IsActive]           BIT            CONSTRAINT [DF_User_IsActive] DEFAULT ((1)) NOT NULL,
    [IsDeleted]          BIT            CONSTRAINT [DF_User_IsDeleted] DEFAULT ((0)) NOT NULL,
    [TokenResetPassword] NVARCHAR (50)  NULL,
    [IsResetPassword]    BIT            CONSTRAINT [DF_User_IsResetPassword] DEFAULT ((0)) NOT NULL,
    [CreatedBy]          INT            NULL,
    [CreatedDate]        DATETIME       NULL,
    [ModifiedBy]         INT            NULL,
    [ModifiedDate]       DATETIME       NULL,
    [SyncedDate]         DATETIME       NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

