using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestParams;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/educationquestion")]
public sealed class EducationQuestionController(
    IEducationQuestionService service)
    : EducationCrudController<EducationQuestionsTable>(service)
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

    [HttpPost("listBySectionId")]
    public async Task<IActionResult> ListBySectionId(
        [FromForm] int sectionId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.ListBySectionAsync(
            sectionId,
            cancellationToken));
    }
}
