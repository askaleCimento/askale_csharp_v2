#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class SMSMessageDto
{
    public int Id { get; set; }

    public int? meetingDetailId { get; set; }

    public string toNumbers { get; set; }

    public string smsText { get; set; }

    public DateTime plannedDate { get; set; }

    public bool isSent { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
