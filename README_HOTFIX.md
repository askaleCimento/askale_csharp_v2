# FormData Liste Hotfix v2

Flutter tarafında `FormData.fromMap(...)` ile çağrılan, fakat backend tarafında `[FromBody]` ile JSON bekleyen liste endpointleri `[FromForm]` olarak düzeltildi.

## Düzeltilen endpointler

1. `helpdeskdemand/talepYonetimiDtoList`
2. `helpdeskstatus/getAllFilter`
3. `helpdeskdemandrule/getAllFilter`
4. `faq/getAllFilter`
5. `usertelephonetable/filterPageableDto`
6. `educationquestionsection/filterByPageable`
7. `education/filterByPageable`
8. `educationsection/filterPageable`

## Kurulum

Paketteki `AskalePortal/Controllers` klasörünü backend projesindeki aynı klasörün üzerine kopyalayın.

Ardından:

```powershell
dotnet clean
dotnet build
dotnet run
```

## Not

`415 Unsupported Media Type` ana backend hatasıdır. Flutter'daki `NoSuchMethodError: data` ise 415 sonrasında boş/null cevap üzerinde mapper çalıştırılmasından doğan ikincil hatadır.
