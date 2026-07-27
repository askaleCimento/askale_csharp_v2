#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class WallPostDto
{
    public int Id { get; set; }

    public int fromUserId { get; set; }

    public string fromUserName { get; set; }

    public int? toUserId { get; set; }

    public string toUserName { get; set; }

    public string text { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
