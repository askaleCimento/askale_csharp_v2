#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class FactoryManagerMeetingDetailDto
{
    public int Id { get; set; }

    public string detailNumber { get; set; }

    public int meetingId { get; set; }

    public int? copyFromMeetingId { get; set; }

    public int? copyFromMeetingDetailId { get; set; }

    public DateTime? meetingDate { get; set; }

    public string title { get; set; }

    public string description { get; set; }

    public string users { get; set; }

    public DateTime? plannedDate { get; set; }

    public DateTime? completedDate { get; set; }

    public string completedNote { get; set; }

    public int itemStatus { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
