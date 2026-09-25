namespace AskalePortal.API.Services.ChatGpt;

public sealed class OpenAiChatOptions
{
    public string ApiKey { get; set; } = "";
    // Public IDs from the Flutter selector; values can be changed to account-supported API IDs.
    public Dictionary<string, string> Models { get; set; } = new()
    {
        ["gpt-4"] = "gpt-4",
        ["gpt-4o"] = "gpt-4o",
        // Compatibility for previously installed Flutter clients.
        ["gpt-5.6-sol"] = "gpt-4o",
        ["gpt-5.6-terra"] = "gpt-4o",
        ["gpt-5.6-luna"] = "gpt-4o",
    };
}
