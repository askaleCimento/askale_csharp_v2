using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/educationvideo")]
public sealed class EducationVideoController(
    IEducationVideoService service)
    : EducationCrudController<EgitimVideoTable>(service)
{
    [HttpPost("filterPageable")]
    [HttpPost("filterByPageable")]
    public async Task<IActionResult> FilterPageable(
        [FromForm] int educationId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListAsync(
            educationId,
            cancellationToken));
    }

    [HttpPost("upload")]
    [HttpPost("uploadVideo")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<IActionResult> UploadVideo(
        [FromForm] List<IFormFile> file,
        [FromForm] int targetId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.UploadVideoAsync(
            file,
            targetId,
            UserId,
            cancellationToken));
    }

    [HttpPost("uploadImage")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<IActionResult> UploadImage(
        [FromForm] List<IFormFile> file,
        [FromForm] int? targetId,
        [FromForm] int? id,
        CancellationToken cancellationToken)
    {
        var effectiveTargetId = targetId ?? id ?? 0;

        return Ok(await service.UploadImageAsync(
            file,
            effectiveTargetId,
            UserId,
            cancellationToken));
    }

    [HttpPost("download")]
    public async Task<IActionResult> Download(
        [FromForm] string file,
        CancellationToken cancellationToken)
    {
        return Ok(await service.DownloadAsync(file, cancellationToken));
    }

    [HttpGet("downloadPicture/{videoId:int}")]
    public async Task<IActionResult> DownloadPicture(
        [FromRoute] int videoId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.DownloadPictureAsync(
            videoId,
            cancellationToken));
    }

    [HttpGet("getVideo/{fileName}")]
    public IActionResult GetVideo([FromRoute] string fileName)
    {
        return PhysicalFile(
            service.GetPath(fileName),
            "application/octet-stream",
            enableRangeProcessing: true);
    }
}
