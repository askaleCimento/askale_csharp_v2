#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRDestinationLocationTableDto
{
    public int Id { get; set; }

    public string destinationLocation { get; set; }

    public int geziYeriId { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
