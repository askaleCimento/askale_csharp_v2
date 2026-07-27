#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HelpDeskMessageDto
{
    public int Id { get; set; }

    public int helpDeskDemandId { get; set; }

    public string message { get; set; }

    public int createdUserId { get; set; }

    public string createdByUserName { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
