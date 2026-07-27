# Frontend–Backend Auth Integration — Step 1

## Endpoints

### POST `/api/auth/login`

```json
{
  "username": "user",
  "password": "password",
  "deviceId": "stable-installation-id"
}
```

### POST `/api/auth/refresh`

```json
{
  "refreshToken": "...",
  "deviceId": "stable-installation-id"
}
```

Refresh token is rotated on every successful refresh. The client must atomically replace both access and refresh tokens.

### POST `/api/auth/logout`

Authorization header is required.

```json
{
  "refreshToken": "...",
  "allSessions": false
}
```

### GET `/api/auth/session`

Returns current user/session claims and roles.

## Frontend mapping

- `accessToken` → secure session access token
- `refreshToken` → secure session refresh token
- `accessTokenExpiresAtUtc` → proactive refresh clock
- `refreshTokenExpiresAtUtc` → restore validity
- `sessionId` → session epoch/scope identity
- HTTP `401` from refresh → centralized session expiry/logout

## Database

Run `database/migrations/001_create_auth_refresh_tokens.sql` before enabling the new endpoints.

## Security behavior

- Refresh tokens are stored only as SHA-256 hashes.
- Every refresh rotates and invalidates the previous token.
- A used, expired, revoked, or wrong-device token is rejected.
- Logout revokes either the current session or all user sessions.
- Access tokens have a separate short lifetime from refresh tokens.
