IF OBJECT_ID(N'dbo.ChatGptConversation', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChatGptConversation
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChatGptConversation PRIMARY KEY,
        createdDate DATETIME NULL,
        createdUserId INT NULL,
        enabled BIT NOT NULL CONSTRAINT DF_ChatGptConversation_enabled DEFAULT (1),
        updatedDate DATETIME NULL,
        updatedUserId INT NULL,
        title NVARCHAR(250) NULL,
        model NVARCHAR(100) NULL
    );

    CREATE INDEX IX_ChatGptConversation_User_Enabled_Updated
        ON dbo.ChatGptConversation(createdUserId, enabled, updatedDate);
END;

IF OBJECT_ID(N'dbo.ChatGptMessage', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChatGptMessage
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChatGptMessage PRIMARY KEY,
        createdDate DATETIME NULL,
        createdUserId INT NULL,
        enabled BIT NOT NULL CONSTRAINT DF_ChatGptMessage_enabled DEFAULT (1),
        updatedDate DATETIME NULL,
        updatedUserId INT NULL,
        conversationId INT NOT NULL,
        role NVARCHAR(20) NOT NULL,
        content NVARCHAR(MAX) NULL,
        messageType NVARCHAR(20) NOT NULL,
        usedToken INT NULL
    );

    CREATE INDEX IX_ChatGptMessage_Conversation_Enabled_Id
        ON dbo.ChatGptMessage(conversationId, enabled, Id);
END;

IF OBJECT_ID(N'dbo.ChatGptAttachment', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChatGptAttachment
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChatGptAttachment PRIMARY KEY,
        createdDate DATETIME NULL,
        createdUserId INT NULL,
        enabled BIT NOT NULL CONSTRAINT DF_ChatGptAttachment_enabled DEFAULT (1),
        updatedDate DATETIME NULL,
        updatedUserId INT NULL,
        messageId INT NOT NULL,
        fileName NVARCHAR(500) NULL,
        contentType NVARCHAR(100) NULL,
        filePath NVARCHAR(1000) NULL,
        attachmentType NVARCHAR(30) NOT NULL
    );

    CREATE INDEX IX_ChatGptAttachment_Message_Enabled_Id
        ON dbo.ChatGptAttachment(messageId, enabled, Id);
END;
