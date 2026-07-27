# Device ID ve IP sözleşmesi

## Login

`POST /api/auth/login` isteği `multipart/form-data` kabul eder.

Alanlar:

- `username`: zorunlu
- `password`: zorunlu
- `ip`: Flutter tarafından best-effort olarak gönderilir
- `deviceId`: Flutter kurulumuna ait kalıcı UUID

## Refresh

`POST /api/auth/refresh` JSON kabul eder:

```json
{
  "refreshToken": "...",
  "ip": "203.0.113.10",
  "deviceId": "8c70b687-44f0-4f31-87d4-889824b285ea"
}
```

Refresh token ilk oluşturulduğunda `DeviceId` ve `CreatedByIp` alanları kaydedilir.
Refresh sırasında token bir cihaz kimliğiyle bağlıysa aynı `deviceId` zorunludur. IP değişikliği tek başına oturumu geçersiz kılmaz; IP audit amacıyla tutulur.
