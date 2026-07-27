# AuthRefreshTokens Cleanup

`RefreshTokenCleanupService` runs once when the API starts and then at the configured interval.

Only old, inactive refresh-token records are physically deleted. A record is eligible when at least one of these timestamps is older than the retention cutoff:

- `ExpiresAtUtc`
- `UsedAtUtc`
- `RevokedAtUtc`

Active tokens are not deleted. Deletion is performed in bounded batches to avoid one large delete transaction and excessive table locking.

## Configuration

```json
{
  "Auth": {
    "RefreshTokenCleanup": {
      "Enabled": true,
      "RetentionDays": 90,
      "IntervalHours": 24,
      "BatchSize": 1000
    }
  }
}
```

- `Enabled`: Enables or disables automatic cleanup.
- `RetentionDays`: Number of days old inactive records are retained for audit.
- `IntervalHours`: Cleanup frequency after the initial startup run.
- `BatchSize`: Maximum rows deleted in each database batch.

No new database migration is required.
