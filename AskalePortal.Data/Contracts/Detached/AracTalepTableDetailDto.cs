#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class AracTalepTableDetailDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public bool? approved { get; set; }

    public Guid guid { get; set; }

    public bool? isReplied { get; set; }

    public DateTime? replyDate { get; set; }

    public int? talepId { get; set; }

    public int? userId { get; set; }

    public int? vekaletUserId { get; set; }

}
