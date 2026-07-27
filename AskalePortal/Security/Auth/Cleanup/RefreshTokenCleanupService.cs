using AskalePortal.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AskalePortal.API.Security.Auth.Cleanup;

public sealed class RefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    IOptions<RefreshTokenCleanupOptions> options,
    TimeProvider timeProvider,
    ILogger<RefreshTokenCleanupService> logger) : BackgroundService
{
    private readonly RefreshTokenCleanupOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            logger.LogInformation("Refresh token cleanup service is disabled.");
            return;
        }

        await CleanupAsync(stoppingToken);

        using var timer = new PeriodicTimer(
            TimeSpan.FromHours(_options.IntervalHours),
            timeProvider);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanupAsync(stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            var cutoffUtc = timeProvider.GetUtcNow().UtcDateTime
                .AddDays(-_options.RetentionDays);
            var totalDeleted = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<DBDataContext>();

                var ids = await db.AuthRefreshTokens
                    .AsNoTracking()
                    .Where(token =>
                        token.ExpiresAtUtc < cutoffUtc ||
                        (token.UsedAtUtc != null && token.UsedAtUtc < cutoffUtc) ||
                        (token.RevokedAtUtc != null && token.RevokedAtUtc < cutoffUtc))
                    .OrderBy(token => token.Id)
                    .Select(token => token.Id)
                    .Take(_options.BatchSize)
                    .ToListAsync(cancellationToken);

                if (ids.Count == 0)
                {
                    break;
                }

                var deleted = await db.AuthRefreshTokens
                    .Where(token => ids.Contains(token.Id))
                    .ExecuteDeleteAsync(cancellationToken);

                totalDeleted += deleted;

                if (ids.Count < _options.BatchSize)
                {
                    break;
                }
            }

            if (totalDeleted > 0)
            {
                logger.LogInformation(
                    "Deleted {DeletedCount} old refresh token records older than {CutoffUtc}.",
                    totalDeleted,
                    cutoffUtc);
            }
            else
            {
                logger.LogDebug(
                    "Refresh token cleanup completed. No records older than {CutoffUtc} were found.",
                    cutoffUtc);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Refresh token cleanup failed.");
        }
    }
}
