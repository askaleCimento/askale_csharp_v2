using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestParams;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/egitimsorulari")]
public sealed class EgitimSorulariController(
    IEgitimSorulariService service)
    : EducationCrudController<EgitimSorulariTable>(service)
{
    [HttpPost("filterPageable")]
    [HttpPost("filterByPageable")]
    public async Task<IActionResult> FilterPageable(
        [FromBody] FilterPageParam<object> request,
        CancellationToken cancellationToken)
    {
        return Ok(await service.FilterPageableAsync(
            request,
            cancellationToken));
    }

    [HttpPost("listByVideoId")]
    public async Task<IActionResult> ListByVideoId(
        [FromForm] int videoId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListByVideoAsync(
            videoId,
            cancellationToken));
    }
}
