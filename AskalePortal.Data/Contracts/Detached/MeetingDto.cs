#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class MeetingDto
{
    public int Id { get; set; }

    public string users { get; set; }

    public DateTime? meetingDate { get; set; }

    public string meetingPlace { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
