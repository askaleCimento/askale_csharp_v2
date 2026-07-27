# Standart API hata sözleşmesi

Tüm yeni API hata cevapları aşağıdaki yapıyı kullanır:

```json
{
  "status": 400,
  "code": "VALIDATION_ERROR",
  "message": "Gönderilen bilgiler geçersiz.",
  "traceId": "00-...",
  "errors": {
    "username": ["The Username field is required."]
  }
}
```

- `status`: HTTP durum kodu.
- `code`: Flutter ve diğer istemcilerin karar vermek için kullanacağı kararlı hata kodu.
- `message`: Kullanıcıya gösterilebilir güvenli mesaj.
- `traceId`: Sunucu loglarıyla istemci hatasını eşleştirmek için kullanılır ve `X-Trace-Id` başlığına da yazılır.
- `errors`: Alan bazlı validation hataları; validation dışındaki hatalarda `null` olabilir.

Standartlaştırılan durumlar:

- 400/422 validation
- 401 authentication
- 403 authorization
- 404 API kaynağı bulunamadı
- 405 desteklenmeyen HTTP metodu
- 409 domain/işlem çakışması (`ApiException` ile)
- 429 rate limit
- 500 beklenmeyen sunucu hatası

Domain veya controller kodundan kontrollü hata üretme örneği:

```csharp
throw new ApiException(
    StatusCodes.Status409Conflict,
    "RECORD_CONFLICT",
    "Kayıt başka bir işlem tarafından değiştirilmiş.");
```
