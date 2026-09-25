using System.Security.Claims;
using System.Text.Json;
using AskalePortal.API.Infrastructure.Errors;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.API.Features.ChatHistory;

[ApiController]
[Authorize]
[Route("api/chatgpt/history")]
[ResponseCache(
    NoStore = true,
    Location = ResponseCacheLocation.None)]
public sealed class ChatHistoryController(
    ChatHistoryDb db,
    DBDataContext legacy,
    ILogger<ChatHistoryController> logger)
    : ControllerBase
{
    private int UserId
    {
        get
        {
            var claim =
                User.FindFirstValue("userId")
                ?? User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return int.TryParse(claim, out var id)
                ? id
                : 0;
        }
    }

    private IQueryable<AiChatSession> Owned =>
        db.Sessions.Where(x =>
            x.OwnerUserId == UserId &&
            !x.Deleted);

    private ObjectResult Error(
        int status,
        string message)
    {
        return StatusCode(
            status,
            ApiErrorWriter.Create(
                HttpContext,
                status,
                "CHAT_HISTORY_ERROR",
                message));
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [FromForm] string? search,
        [FromForm] int offset,
        CancellationToken ct)
    {
        if (UserId <= 0)
        {
            return Unauthorized();
        }

        offset = Math.Clamp(offset, 0, 10000);
        search = (search ?? string.Empty).Trim();

        if (search.Length > 160)
        {
            return Error(
                400,
                "Arama metni en fazla 160 karakter olabilir.");
        }

        try
        {
            var sessionsQuery = Owned.AsNoTracking();

            if (search.Length > 0)
            {
                sessionsQuery = sessionsQuery.Where(x =>
                    x.Title.Contains(search));
            }

            var sessions = await sessionsQuery
                .OrderByDescending(x => x.UpdatedAt)
                .ThenBy(x => x.Id)
                .Skip(offset)
                .Take(51)
                .Select(x => new
                {
                    id = x.Id.ToString(),
                    title = x.Title,
                    model = x.Model,
                    updatedAt = x.UpdatedAt,
                    version = x.Version
                })
                .ToListAsync(ct);

            var oldQueries = legacy.ChatGptQueries
                .AsNoTracking()
                .Where(x =>
                    x.createdUserId == UserId &&
                    x.enabled);

            if (search.Length > 0)
            {
                oldQueries = oldQueries.Where(x =>
                    x.query.Contains(search));
            }

            var searches = await oldQueries
                .OrderByDescending(x => x.Id)
                .Skip(offset)
                .Take(51)
                .Select(x => new
                {
                    id = x.Id,
                    title = x.query,
                    updatedAt = x.createdDate
                })
                .ToListAsync(ct);

            return Ok(new
            {
                sessions = sessions.Take(50),
                searches = searches.Take(50),
                hasMore =
                    sessions.Count > 50 ||
                    searches.Count > 50
            });
        }
        catch (Exception error)
        {
            logger.LogError(
                error,
                "Chat history list failed. Database: {Database}",
                db.Database.GetDbConnection().Database);

            return Error(
                500,
                "Sohbet geçmişi okunamadı.");
        }
    }

    [HttpPost("get")]
    public async Task<IActionResult> Get(
        [FromForm] Guid id,
        CancellationToken ct)
    {
        if (UserId <= 0)
        {
            return Unauthorized();
        }

        try
        {
            var session = await Owned
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.Id == id,
                    ct);

            if (session is null)
            {
                return NotFound();
            }

            return Ok(BuildResponse(session));
        }
        catch (Exception error)
        {
            logger.LogError(
                error,
                "Chat history get failed. Id: {Id}, Database: {Database}",
                id,
                db.Database.GetDbConnection().Database);

            return Error(
                500,
                "Sohbet açılamadı.");
        }
    }

    [HttpPost("save")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(32 * 1024 * 1024)]
    [RequestFormLimits(
        ValueLengthLimit = 31 * 1024 * 1024,
        MultipartBodyLengthLimit = 32 * 1024 * 1024)]
    public async Task<IActionResult> Save(
        [FromForm] string? request,
        CancellationToken ct)
    {
        if (UserId <= 0)
        {
            return Unauthorized();
        }

        SaveChatSession data;

        try
        {
            if (string.IsNullOrWhiteSpace(request))
            {
                throw new FormatException(
                    "Sohbet bilgisi eksik.");
            }

            data =
                JsonSerializer.Deserialize<SaveChatSession>(
                    request,
                    ChatHistoryContract.Json)
                ?? throw new FormatException(
                    "Sohbet bilgisi eksik.");

            ChatHistoryContract.Validate(data);
        }
        catch (JsonException error)
        {
            logger.LogWarning(
                error,
                "Invalid chat history JSON.");

            return Error(
                400,
                "Sohbet verisi geçersiz.");
        }
        catch (FormatException error)
        {
            return Error(
                400,
                error.Message);
        }

        try
        {
            var transcript =
                JsonSerializer.Serialize(
                    data.Messages,
                    ChatHistoryContract.Json);

            var session =
                await db.Sessions
                    .SingleOrDefaultAsync(
                        x => x.Id == data.Id,
                        ct);

            if (session is not null &&
                (session.OwnerUserId != UserId ||
                 session.Deleted))
            {
                return NotFound();
            }

            if (session is null)
            {
                if (data.Version != 0)
                {
                    return Error(
                        409,
                        "Sohbet başka yerde değişmiş. Yeniden açın.");
                }

                var activeCount =
                    await Owned.CountAsync(ct);

                if (activeCount >= 500)
                {
                    return Error(
                        400,
                        "Sohbet sınırına ulaştınız.");
                }

                var title =
                    data.Messages
                        .FirstOrDefault()
                        ?.Message
                        .Trim()
                    ?? "Yeni sohbet";

                if (title.Length > 160)
                {
                    title = title[..160];
                }

                session = new AiChatSession
                {
                    Id = data.Id,
                    OwnerUserId = UserId,
                    Title = title,
                    Model = data.Model,
                    Transcript = transcript,
                    Version = 1,
                    Deleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                db.Sessions.Add(session);
            }
            else
            {
                // Aynı istek tekrar geldiyse ikinci kez kayıt oluşturma.
                if (session.Transcript == transcript &&
                    session.Model == data.Model)
                {
                    return Ok(BuildResponse(session));
                }

                if (session.Version != data.Version)
                {
                    return Error(
                        409,
                        "Bu sohbet başka bir sekmede değişti. Yeniden açın.");
                }

                if (session.Model != data.Model)
                {
                    return Error(
                        400,
                        "Model değiştirmek için yeni sohbet açın.");
                }

                session.Transcript = transcript;
                session.Version++;
                session.UpdatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(ct);

            return Ok(BuildResponse(session));
        }
        catch (DbUpdateException error)
        {
            logger.LogError(
                error,
                "Chat history database save failed. Database: {Database}",
                db.Database.GetDbConnection().Database);

            return Error(
                500,
                "Sohbet veritabanına kaydedilemedi.");
        }
        catch (Exception error)
        {
            logger.LogError(
                error,
                "Chat history save failed. Database: {Database}",
                db.Database.GetDbConnection().Database);

            return Error(
                500,
                "Sohbet kaydedilirken sunucu hatası oluştu.");
        }
    }

    [HttpPost("rename")]
    public async Task<IActionResult> Rename(
        [FromForm] Guid id,
        [FromForm] string title,
        CancellationToken ct)
    {
        if (UserId <= 0)
        {
            return Unauthorized();
        }

        title = (title ?? string.Empty).Trim();

        if (title.Length is < 1 or > 160)
        {
            return Error(
                400,
                "Başlık 1-160 karakter arasında olmalıdır.");
        }

        try
        {
            var session = await Owned
                .SingleOrDefaultAsync(
                    x => x.Id == id,
                    ct);

            if (session is null)
            {
                return NotFound();
            }

            session.Title = title;
            session.Version++;
            session.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);

            return Ok(BuildResponse(session));
        }
        catch (DbUpdateConcurrencyException error)
        {
            logger.LogWarning(
                error,
                "Chat rename concurrency conflict. Id: {Id}",
                id);

            return Error(
                409,
                "Sohbet başka yerde değişti. Yeniden açın.");
        }
        catch (Exception error)
        {
            logger.LogError(
                error,
                "Chat rename failed. Id: {Id}",
                id);

            return Error(
                500,
                "Sohbet yeniden adlandırılamadı.");
        }
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(
        [FromForm] Guid id,
        CancellationToken ct)
    {
        if (UserId <= 0)
        {
            return Unauthorized();
        }

        try
        {
            var session = await Owned
                .SingleOrDefaultAsync(
                    x => x.Id == id,
                    ct);

            if (session is null)
            {
                return NotFound();
            }

            session.Deleted = true;
            session.Title = string.Empty;
            session.Transcript = "[]";
            session.Version++;
            session.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);

            return Ok(new
            {
                deleted = true
            });
        }
        catch (DbUpdateConcurrencyException error)
        {
            logger.LogWarning(
                error,
                "Chat delete concurrency conflict. Id: {Id}",
                id);

            return Error(
                409,
                "Sohbet başka yerde değişti. Yeniden açın.");
        }
        catch (Exception error)
        {
            logger.LogError(
                error,
                "Chat delete failed. Id: {Id}",
                id);

            return Error(
                500,
                "Sohbet silinemedi.");
        }
    }

    private static object BuildResponse(
        AiChatSession session)
    {
        List<SavedChatMessage>? messages;

        try
        {
            messages =
                JsonSerializer.Deserialize<
                    List<SavedChatMessage>>(
                    session.Transcript,
                    ChatHistoryContract.Json);
        }
        catch
        {
            messages = [];
        }

        return new
        {
            id = session.Id.ToString(),
            title = session.Title,
            model = session.Model,
            version = session.Version,
            updatedAt = session.UpdatedAt,
            messages = ChatHistoryResponse.Messages(messages ?? [])
        };
    }
}
