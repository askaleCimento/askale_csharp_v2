#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class DigitalCoridorUserTableDto
{
    public int Id { get; set; }

    public string userName { get; set; }

    public string location { get; set; }

    public int? sectionId { get; set; }

    public string email { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
