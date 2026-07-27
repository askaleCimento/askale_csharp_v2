using System.Diagnostics;

namespace AskalePortal.API.Infrastructure.Errors;

public static class ApiErrorWriter
{
    public static ApiErrorResponse Create(
        HttpContext context,
        int status,
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        context.Response.Headers["X-Trace-Id"] = traceId;
        return new ApiErrorResponse(status, code, message, traceId, errors);
    }

    public static async Task WriteAsync(
        HttpContext context,
        int status,
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null,
        CancellationToken cancellationToken = default)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(
            Create(context, status, code, message, errors),
            cancellationToken: cancellationToken);
    }
}
