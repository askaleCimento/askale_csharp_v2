using System.Security.Claims;
using System.Text.Json;
using AskalePortal.API.Features.ChatHistory;
using AskalePortal.API.Infrastructure.Errors;
using AskalePortal.API.Services.ChatGpt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AskalePortal.API.Controllers;

[ApiController, Authorize, Route("api/chatgpt")]
public sealed class ChatCompletionController(OpenAiResponsesService service) : ControllerBase
{
    [HttpPost("complete"), EnableRateLimiting("chat-gpt")]
    [RequestSizeLimit(32 * 1024 * 1024)]
    [RequestFormLimits(ValueLengthLimit = 32 * 1024 * 1024, MultipartBodyLengthLimit = 32 * 1024 * 1024)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Complete([FromForm] string request, CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) || id <= 0)
            return Unauthorized();
        try
        {
            var chat = JsonSerializer.Deserialize<SaveChatSession>(request, ChatHistoryContract.Json);
            if (chat is null) throw new JsonException();
            var reply = await service.CompleteAsync(chat, ct);
            // Content avoids Newtonsoft.Json interpreting System.Text.Json.JsonElement.
            return Content(reply.GetRawText(), "application/json; charset=utf-8");
        }
        catch (JsonException)
        { return StatusCode(400, ApiErrorWriter.Create(HttpContext, 400, "CHAT_INVALID_REQUEST", "Mesaj bilgisi geçerli JSON biçiminde olmalıdır.")); }
        catch (ChatServiceException ex)
        { return StatusCode(ex.Status, ApiErrorWriter.Create(HttpContext, ex.Status, ex.Code, ex.Message)); }
    }
}
