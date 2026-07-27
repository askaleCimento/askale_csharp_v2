#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class IcYazismalarMesajTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public int? icYazismaId { get; set; }

    public string message { get; set; }

    public int? sendUserId { get; set; }

    public bool? showAll { get; set; }

    public int? userId { get; set; }

}
