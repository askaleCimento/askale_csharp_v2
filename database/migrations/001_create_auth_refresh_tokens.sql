IF OBJECT_ID(N'dbo.AuthRefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuthRefreshTokens
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuthRefreshTokens PRIMARY KEY,
        UserId INT NOT NULL,
        TokenHash NVARCHAR(64) NOT NULL,
        JwtId NVARCHAR(64) NOT NULL,
        SessionId NVARCHAR(64) NOT NULL,
        DeviceId NVARCHAR(200) NULL,
        CreatedAtUtc DATETIME2 NOT NULL,
        ExpiresAtUtc DATETIME2 NOT NULL,
        UsedAtUtc DATETIME2 NULL,
        RevokedAtUtc DATETIME2 NULL,
        RevokedReason NVARCHAR(200) NULL,
        ReplacedByTokenHash NVARCHAR(64) NULL,
        CreatedByIp NVARCHAR(64) NULL,
        UserAgent NVARCHAR(512) NULL,
        CONSTRAINT FK_AuthRefreshTokens_AdminUser_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.AdminUser(Id)
    );

    CREATE UNIQUE INDEX UX_AuthRefreshTokens_TokenHash
        ON dbo.AuthRefreshTokens(TokenHash);
    CREATE INDEX IX_AuthRefreshTokens_UserId_SessionId
        ON dbo.AuthRefreshTokens(UserId, SessionId);
    CREATE INDEX IX_AuthRefreshTokens_ExpiresAtUtc
        ON dbo.AuthRefreshTokens(ExpiresAtUtc);
END;
