#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class IPAddressDto
{
    public int Id { get; set; }

    public string title { get; set; }

    public string iP { get; set; }

    public bool isBlocked { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
