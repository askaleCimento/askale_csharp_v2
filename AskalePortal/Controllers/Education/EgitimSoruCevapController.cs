using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/egitimsorucevap")]
public sealed class EgitimSoruCevapController(
    IEgitimSoruCevapService service)
    : EducationCrudController<EgitimSoruCevap>(service)
{
    [HttpPost("listByVideoId")]
    public async Task<IActionResult> ListByVideoId(
        [FromForm] int videoId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListAsync(
            videoId,
            null,
            cancellationToken));
    }

    [HttpPost("listByVideoIdAndUserId")]
    public async Task<IActionResult> ListByVideoIdAndUserId(
        [FromForm] int videoId,
        [FromForm] int userId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListAsync(
            videoId,
            userId,
            cancellationToken));
    }
}
