using AskalePortal.BLL.Education;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModels.Education;
using AskalePortal.Data.RequestParams;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers.Education;

[Route("api/educationquestionsection")]
public sealed class EducationQuestionSectionController(
    IEducationQuestionSectionService service)
    : EducationCrudController<EducationQuestionSectionTable>(service)
{
    [HttpPost("filterPageable")]
    [HttpPost("filterByPageable")]
    public async Task<IActionResult> FilterPageable(
        [FromBody] FilterPageParam<EducationQuestionSectionSearchRequest> request,
        CancellationToken cancellationToken)
    {
        return Ok(await service.FilterAsync(request, cancellationToken));
    }
}
