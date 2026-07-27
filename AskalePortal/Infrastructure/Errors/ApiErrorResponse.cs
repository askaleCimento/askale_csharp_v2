namespace AskalePortal.API.Infrastructure.Errors;

public sealed record ApiErrorResponse(
    int Status,
    string Code,
    string Message,
    string TraceId,
    IReadOnlyDictionary<string, string[]>? Errors = null);
