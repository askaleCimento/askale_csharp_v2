using AskalePortal.Data.Contracts.Detached;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AskalePortal.API.Infrastructure.Serialization;

public sealed class DetachedEntityResultFilter : IAsyncResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
        {
            objectResult.Value = DetachedDtoMapper.ToDetached(objectResult.Value);
            objectResult.DeclaredType = objectResult.Value?.GetType();
        }
        return next();
    }
}
