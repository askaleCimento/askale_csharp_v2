using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.API.Controllers;

[ApiController, Authorize, Route("api/chatgpt")]
public sealed class ChatGptController(DBDataContext db) : ControllerBase
{
    // Legacy Flutter posts entity.toJson() here, just as it did to Java.
    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] ChatGptSaveRequest request, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();
        ChatGptQueries entity;
        if (request.Id is > 0)
        {
            var existing = await db.ChatGptQueries.SingleOrDefaultAsync(
                x => x.Id == request.Id && x.createdUserId == userId, ct);
            if (existing is null) return NotFound();
            entity = existing;
            entity.updatedDate = DateTime.Now;
            entity.updatedUserId = userId;
        }
        else
        {
            entity = new ChatGptQueries { createdDate = DateTime.Now, createdUserId = userId, enabled = true };
            db.ChatGptQueries.Add(entity);
        }
        entity.query = request.Query!.Length > 255 ? request.Query[..255] : request.Query;
        entity.usedToken = request.UsedToken;
        await db.SaveChangesAsync(ct);
        return Ok(ToResponse(entity));
    }

    [HttpPost("getById")]
    public async Task<IActionResult> GetById([FromForm] int id, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();
        var entity = await db.ChatGptQueries.AsNoTracking().SingleOrDefaultAsync(
            x => x.Id == id && x.createdUserId == userId, ct);
        return entity is null ? NotFound() : Ok(ToResponse(entity));
    }

    private int CurrentUserId() => int.TryParse(
        User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier),
        out var id) ? id : 0;

    private static object ToResponse(ChatGptQueries entity) => new {
        id = entity.Id, entity.enabled, entity.createdUserId, entity.createdDate,
        updatedUserId = entity.updatedUserId, updateDate = entity.updatedDate,
        entity.query, entity.usedToken
    };
}

public sealed class ChatGptSaveRequest
{
    public int? Id { get; set; }
    [Required, StringLength(16000)]
    public string? Query { get; set; }
    [Range(0, int.MaxValue)]
    public int? UsedToken { get; set; }
}
