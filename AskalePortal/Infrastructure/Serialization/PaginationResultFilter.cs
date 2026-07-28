using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseParams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AskalePortal.API.Infrastructure.Serialization;

/// <summary>
/// Normalizes pagination metadata returned by legacy BLL methods.
/// Many legacy methods only fill content/totalElements/size, while the Flutter
/// footer expects pageable.pageNumber and totalPages to be present.
/// </summary>
public sealed class PaginationResultFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var paginationRequest = context.ActionArguments.Values
            .OfType<IPaginationRequest>()
            .FirstOrDefault();

        var executedContext = await next();

        if (paginationRequest is null ||
            (executedContext.Exception is not null && !executedContext.ExceptionHandled))
        {
            return;
        }

        object? responseValue = executedContext.Result switch
        {
            ObjectResult objectResult => objectResult.Value,
            JsonResult jsonResult => jsonResult.Value,
            _ => null
        };

        if (responseValue is not IPaginationResult paginationResult)
        {
            return;
        }

        paginationResult.NormalizePagination(
            paginationRequest.Page ?? 0,
            paginationRequest.Size ?? 0);
    }
}
