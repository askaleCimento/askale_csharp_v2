using Microsoft.EntityFrameworkCore;

namespace AskalePortal.Data.Models;

public partial class DBDataContext
{
    public DbSet<AuthRefreshToken> AuthRefreshTokens => Set<AuthRefreshToken>();

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
    }
}
