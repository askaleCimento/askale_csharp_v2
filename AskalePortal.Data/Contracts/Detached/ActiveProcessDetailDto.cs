#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ActiveProcessDetailDto
{
    public int Id { get; set; }

    public Guid guid { get; set; }

    public int activeProcessId { get; set; }

    public int userId { get; set; }

    public int? vekaletId { get; set; }

    public bool? approved { get; set; }

    public DateTime createdDate { get; set; }

    public bool isReplied { get; set; }

    public DateTime? replyDate { get; set; }

    public string description { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
