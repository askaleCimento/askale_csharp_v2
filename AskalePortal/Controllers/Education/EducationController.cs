using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModels.Education;
using AskalePortal.Data.RequestParams;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/education")]
public sealed class EducationController(
    IEducationService service)
    : EducationCrudController<EgitimTable>(service)
{
    [HttpPost("filterPageable")]
    [HttpPost("filterByPageable")]
    public async Task<IActionResult> FilterPageable(
        [FromBody] FilterPageParam<EducationSearchRequest> request,
        CancellationToken cancellationToken)
    {
        return Ok(await service.FilterAsync(request, cancellationToken));
    }

    [HttpPost("listByEgitimBolumId")]
    public async Task<IActionResult> ListByEgitimBolumId(
        [FromForm] int egitimBolumId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListBySectionAsync(
            egitimBolumId,
            cancellationToken));
    }
}
