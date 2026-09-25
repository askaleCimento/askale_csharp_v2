using Microsoft.EntityFrameworkCore;

namespace AskalePortal.API.Features.ChatHistory;

public sealed class ChatHistoryDb
    : DbContext
{
    public ChatHistoryDb(
        DbContextOptions<ChatHistoryDb> options)
        : base(options)
    {
    }

    public DbSet<AiChatSession> Sessions =>
        Set<AiChatSession>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entity = modelBuilder.Entity<AiChatSession>();

        entity.ToTable(
            "AiChatSessions",
            "dbo");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Title)
            .HasMaxLength(160)
            .IsRequired();

        entity.Property(x => x.Model)
            .HasMaxLength(80)
            .IsRequired();

        entity.Property(x => x.Transcript)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        entity.Property(x => x.Version)
            .IsRequired()
            .IsConcurrencyToken();

        entity.Property(x => x.Deleted)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.Property(x => x.UpdatedAt)
            .IsRequired();

        entity.HasIndex(x => new
        {
            x.OwnerUserId,
            x.Deleted,
            x.UpdatedAt
        })
        .HasDatabaseName(
            "IX_AiChatSessions_OwnerUserId_Deleted_UpdatedAt");
    }
}

public sealed class AiChatSession
{
    public Guid Id { get; set; }

    public int OwnerUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string Transcript { get; set; } = "[]";

    public int Version { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public static class ChatHistoryRegistration
{
    public static IServiceCollection AddChatHistory(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var mode =
            Environment.GetEnvironmentVariable(
                "ASKALE_ENVIRONMENT")
            ?.ToLowerInvariant()
            ?? (
                environment.IsProduction()
                    ? "server"
                    : environment.IsDevelopment()
                        ? "local"
                        : "test"
            );

        var connectionString =
            configuration[$"ConnectionStrings:{mode}"]
            ?? configuration.GetConnectionString(mode);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"'{mode}' baðlantý ayarý bulunamadý.");
        }

        services.AddDbContext<ChatHistoryDb>(
            options =>
            {
                options.UseSqlServer(
                    connectionString,
                    sql =>
                    {
                        sql.UseCompatibilityLevel(120);
                    });
            });

        return services;
    }
}