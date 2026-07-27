#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRTripDescriptionDto
{
    public int Id { get; set; }

    public int sapId { get; set; }

    public string tripDescription { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
