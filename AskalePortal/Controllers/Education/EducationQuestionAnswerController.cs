using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/educationquestionanswer")]
public sealed class EducationQuestionAnswerController(
    IEducationQuestionAnswerService service)
    : EducationCrudController<EducationQuestionAnswerTable>(service)
{
    [HttpPost("listBySectionId")]
    public async Task<IActionResult> ListBySectionId(
        [FromForm] int sectionId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListAsync(
            sectionId,
            null,
            cancellationToken));
    }

    [HttpPost("listBySectionIdAndUserId")]
    public async Task<IActionResult> ListBySectionIdAndUserId(
        [FromForm] int sectionId,
        [FromForm] int userId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListAsync(
            sectionId,
            userId,
            cancellationToken));
    }
}
