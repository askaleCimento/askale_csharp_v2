namespace AskalePortal.API.Features.ChatHistory;

public static class ChatHistoryResponse
{
    // DetachedEntityResultFilter converts DTOs to dictionaries before Newtonsoft
    // serialization. Dictionary keys retain their original case, so define the
    // wire names explicitly at every level instead of returning SavedChatMessage.
    public static object[] Messages(IEnumerable<SavedChatMessage> messages) =>
        messages.Select(m => (object)new {
            user = m.User,
            message = m.Message,
            tokens = m.Tokens,
            attachments = m.Attachments.Select(f => new {
                name = f.Name,
                mimeType = f.MimeType,
                bytes = Convert.ToBase64String(f.Bytes)
            }).ToArray()
        }).ToArray();
}
