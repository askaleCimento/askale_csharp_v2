using Microsoft.AspNetCore.Diagnostics;

namespace AskalePortal.API.Infrastructure.Errors;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            return false;
        }

        (int status, string code, string message, IReadOnlyDictionary<string, string[]>? errors) = exception switch
        {
            ApiException apiException => (
                apiException.StatusCode,
                apiException.Code,
                apiException.Message,
                apiException.Errors),
            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "INVALID_REQUEST",
                "İstek biçimi geçersiz.",
                null),
            OperationCanceledException when httpContext.RequestAborted.IsCancellationRequested => (
                499,
                "REQUEST_CANCELLED",
                "İstek istemci tarafından iptal edildi.",
                null),
            _ => (
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "Beklenmeyen bir sunucu hatası oluştu.",
                null)
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled API exception. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(
                exception,
                "API request failed with {StatusCode}. TraceId: {TraceId}",
                status,
                httpContext.TraceIdentifier);
        }

        var responseMessage = status >= 500 && environment.IsDevelopment()
            ? $"{message} {exception.Message}"
            : message;

        await ApiErrorWriter.WriteAsync(
            httpContext,
            status,
            code,
            responseMessage,
            errors,
            cancellationToken);

        return true;
    }
}
