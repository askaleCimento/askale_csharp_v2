#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EmailMessageDto
{
    public int Id { get; set; }

    public int? meetingDetailId { get; set; }

    public string toAddress { get; set; }

    public string subject { get; set; }

    public string emailText { get; set; }

    public DateTime plannedDate { get; set; }

    public bool isSent { get; set; }

    public int? mailTuru { get; set; }

    public string dosya { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
