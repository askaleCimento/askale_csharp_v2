#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRAccountJobTableDto
{
    public int Id { get; set; }

    public int tripId { get; set; }

    public bool isSent { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
