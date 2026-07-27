#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRUserLogDto
{
    public int Id { get; set; }

    public int userId { get; set; }

    public int? changedId { get; set; }

    public int? changingUserId { get; set; }

    public int? changedUserId { get; set; }

    public DateTime changeDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
