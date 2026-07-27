using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModels.Education;
using AskalePortal.Data.RequestParams;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/educationsection")]
public sealed class EducationSectionController(
    IEducationSectionService service)
    : EducationCrudController<EgitimBolumTable>(service)
{
    [HttpPost("filterPageable")]
    [HttpPost("filterByPageable")]
    public async Task<IActionResult> FilterPageable(
        [FromBody] FilterPageParam<EducationSectionSearchRequest> request,
        CancellationToken cancellationToken)
    {
        return Ok(await service.FilterAsync(request, cancellationToken));
    }
}
