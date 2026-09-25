using Microsoft.EntityFrameworkCore;

namespace AskalePortal.Data.Models;

public partial class DBDataContext
{
    public DbSet<AuthRefreshToken> AuthRefreshTokens => Set<AuthRefreshToken>();
    public DbSet<AuthPasswordReset> AuthPasswordResets => Set<AuthPasswordReset>();
    public DbSet<ChatGptConversation> ChatGptConversation => Set<ChatGptConversation>();
    public DbSet<ChatGptMessage> ChatGptMessage => Set<ChatGptMessage>();
    public DbSet<ChatGptAttachment> ChatGptAttachment => Set<ChatGptAttachment>();

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthRefreshToken>(entity =>
        {
            entity.ToTable("AuthRefreshTokens");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(x => x.JwtId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.SessionId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.DeviceId).HasMaxLength(200);
            entity.Property(x => x.RevokedReason).HasMaxLength(200);
            entity.Property(x => x.ReplacedByTokenHash).HasMaxLength(64);
            entity.Property(x => x.CreatedByIp).HasMaxLength(64);
            entity.Property(x => x.UserAgent).HasMaxLength(512);
            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => new { x.UserId, x.SessionId });
            entity.HasIndex(x => x.ExpiresAtUtc);
        });

        modelBuilder.Entity<AuthPasswordReset>(entity =>
        {
            entity.ToTable("AuthPasswordResets");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(x => x.PasswordSnapshot).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.ProtectedSecret).HasMaxLength(512);
            entity.HasIndex(x => x.ExpiresAtUtc);
            entity.HasIndex(x => x.NextSendAtUtc);
        });

        modelBuilder.Entity<ChatGptConversation>(entity =>
        {
            entity.ToTable("ChatGptConversation");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.createdDate).HasColumnType("datetime");
            entity.Property(e => e.updatedDate).HasColumnType("datetime");
            entity.Property(e => e.title).HasMaxLength(250);
            entity.Property(e => e.model).HasMaxLength(100);
            entity.HasIndex(e => new { e.createdUserId, e.enabled, e.updatedDate });
        });

        modelBuilder.Entity<ChatGptMessage>(entity =>
        {
            entity.ToTable("ChatGptMessage");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.createdDate).HasColumnType("datetime");
            entity.Property(e => e.updatedDate).HasColumnType("datetime");
            entity.Property(e => e.role).HasMaxLength(20).IsRequired();
            entity.Property(e => e.content).HasColumnType("nvarchar(max)");
            entity.Property(e => e.messageType).HasMaxLength(20).IsRequired();
            entity.HasIndex(e => new { e.conversationId, e.enabled, e.Id });
        });

        modelBuilder.Entity<ChatGptAttachment>(entity =>
        {
            entity.ToTable("ChatGptAttachment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.createdDate).HasColumnType("datetime");
            entity.Property(e => e.updatedDate).HasColumnType("datetime");
            entity.Property(e => e.fileName).HasMaxLength(500);
            entity.Property(e => e.contentType).HasMaxLength(100);
            entity.Property(e => e.filePath).HasMaxLength(1000);
            entity.Property(e => e.attachmentType).HasMaxLength(30).IsRequired();
            entity.HasIndex(e => new { e.messageId, e.enabled, e.Id });
        });
    }
}
