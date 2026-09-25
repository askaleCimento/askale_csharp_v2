using System.Net;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using AskalePortal.API.Controllers;
using AskalePortal.API.Features.ChatHistory;
using AskalePortal.API.Services.ChatGpt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AskalePortal.Data.Contracts.Detached;

int passed = 0;
void Check(bool value, string name) { if (!value) throw new Exception(name); Console.WriteLine("PASS " + name); passed++; }
var historyMessages = new List<SavedChatMessage> { new() { User = "user", Message = "selam", Attachments = [
    new() { Name = "a.txt", MimeType = "text/plain", Bytes = [79,75] }] } };
var historyJson = JsonSerializer.SerializeToElement(DetachedDtoMapper.ToDetached(new { messages = ChatHistoryResponse.Messages(historyMessages) }));
var historyMessage = historyJson.GetProperty("messages")[0];
Check(historyMessage.GetProperty("user").GetString() == "user" && historyMessage.GetProperty("message").GetString() == "selam", "history lowercase message keys after global filter");
Check(historyMessage.GetProperty("attachments")[0].GetProperty("mimeType").GetString() == "text/plain" && historyMessage.GetProperty("attachments")[0].GetProperty("bytes").GetString() == "T0s=", "history lowercase attachment keys and base64");
SaveChatSession Request() => new() { Id = Guid.NewGuid(), Model = "gpt-5.6-luna", Messages = [new() { User = "user", Message = "Merhaba" }] };
const string success = """{"status":"completed","output":[{"type":"reasoning"},{"content":[{"type":"output_text","text":"Merhaba!"}]}],"usage":{"output_tokens":4}}""";
var handler = new StubHandler(200, success);
OpenAiResponsesService Service(StubHandler h, string key = "test-key") => new(new HttpClient(h), Options.Create(new OpenAiChatOptions { ApiKey = key }));
var service = Service(handler);
var answer = await service.CompleteAsync(Request(), default);
Check(answer.GetProperty("output_text").GetString() == "Merhaba!", "response text");
Check(answer.GetProperty("usage").GetProperty("output_tokens").GetInt32() == 4, "tokens");
using (var sent = JsonDocument.Parse(handler.Body!)) {
    Check(handler.Url == "https://api.openai.com/v1/responses" && handler.Auth == "Bearer test-key", "endpoint and server key");
    Check(sent.RootElement.GetProperty("model").GetString() == "gpt-5.6-luna" && !sent.RootElement.GetProperty("store").GetBoolean(), "model and no storage");
}
var files = Request();
files.Model = "gpt-5.6-luna";
files.Messages[0].Attachments = [new() { Name = "a.txt", Bytes = Encoding.UTF8.GetBytes("example") }, new() { Name = "a.png", Bytes = [137,80,78,71,13,10,26,10] }];
await service.CompleteAsync(files, default);
using (var sent = JsonDocument.Parse(handler.Body!)) {
    var content = sent.RootElement.GetProperty("input")[0].GetProperty("content");
    Check(content[1].GetProperty("type").GetString() == "input_file" && content[2].GetProperty("type").GetString() == "input_image", "file and image preserved");
    Check(sent.RootElement.GetProperty("model").GetString() == "gpt-5.6-luna", "legacy alias");
}
async Task Error(OpenAiResponsesService s, SaveChatSession request, string code) {
    try { await s.CompleteAsync(request, default); throw new Exception("Expected " + code); }
    catch (ChatServiceException ex) { Check(ex.Code == code, code); }
}
await Error(Service(new(200, success), ""), Request(), "CHAT_NOT_CONFIGURED");
await Error(Service(new(401, "{}")), Request(), "CHAT_UPSTREAM_AUTH");
await Error(Service(new(429, """{"error":{"code":"insufficient_quota"}}""")), Request(), "CHAT_QUOTA_EXHAUSTED");
await Error(Service(new(429, "{}")), Request(), "CHAT_UPSTREAM_LIMIT");
await Error(Service(new(404, "{}")), Request(), "CHAT_UPSTREAM_MODEL");
await Error(Service(new(500, "not json")), Request(), "CHAT_UPSTREAM_ERROR");
await Error(Service(new(200, "{}")), Request(), "CHAT_INVALID_REPLY");
await Error(Service(new(200, """{"status":"incomplete","output":[]} """)), Request(), "CHAT_INVALID_REPLY");
await Error(Service(new(200, "[]")), Request(), "CHAT_INVALID_REPLY");
var invalid = Request(); invalid.Messages[0].User = "system";
await Error(service, invalid, "CHAT_INVALID_REQUEST");
var context = new DefaultHttpContext();
context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("userId", "9")], "test"));
var controller = new ChatCompletionController(service) { ControllerContext = new() { HttpContext = context } };
var result = await controller.Complete(JsonSerializer.Serialize(Request(), ChatHistoryContract.Json), default);
Check(result is ContentResult { ContentType: "application/json; charset=utf-8" } contentResult && contentResult.Content!.Contains("output_text"), "MVC JSON response");
Check(await controller.Complete("{", default) is ObjectResult { StatusCode: 400 }, "invalid form JSON");
context.User = new ClaimsPrincipal();
Check(await controller.Complete("{}", default) is UnauthorizedResult, "requires user identity");
Console.WriteLine($"{passed} checks passed.");

sealed class StubHandler(int status, string body) : HttpMessageHandler {
    public string? Body, Url, Auth;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) {
        Body = await request.Content!.ReadAsStringAsync(ct); Url = request.RequestUri!.ToString(); Auth = request.Headers.Authorization!.ToString();
        return new((HttpStatusCode)status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
    }
}
