#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRAnnouncementDto
{
    public int Id { get; set; }

    public string title { get; set; }

    public string description { get; set; }

    public string imageUrl { get; set; }

    public DateTime createdDate { get; set; }

    public int createdUserId { get; set; }

    public string createdByUserName { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
