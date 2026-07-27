using AskalePortal.BLL.Education;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers.Education;

[ApiController]
public abstract class EducationCrudController<T>(
    IEducationCrudService<T> service) : ControllerBase
    where T : class
{
    protected readonly IEducationCrudService<T> Service = service;

    protected int UserId
    {
        get
        {
            var value = User.FindFirstValue("userId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            return int.TryParse(value, out var id) ? id : 0;
        }
    }

    [HttpPost("save")]
    public async Task<ActionResult<T>> Save(
        [FromBody] T entity,
        CancellationToken cancellationToken)
    {
        var saved = await Service.SaveAsync(
            entity,
            UserId,
            cancellationToken);

        return Ok(saved);
    }

    [HttpPost("delete")]
    public async Task<ActionResult<int>> Delete(
        [FromForm] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await Service.DeleteAsync(id, cancellationToken));
    }

    [HttpPost("getById")]
    public async Task<ActionResult<T>> GetById(
        [FromForm] int id,
        CancellationToken cancellationToken)
    {
        var row = await Service.GetByIdAsync(id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost("getAll")]
    public async Task<ActionResult<List<T>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await Service.GetAllAsync(cancellationToken));
    }

    [HttpPost("getAllFilter")]
    public async Task<IActionResult> GetAllFilter(
        [FromBody] object? request,
        CancellationToken cancellationToken)
    {
        return Ok(await Service.GetAllFilterAsync(request, cancellationToken));
    }
}
