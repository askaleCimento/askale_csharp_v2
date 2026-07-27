#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class MusteriSikayetTipiDto
{
    public int Id { get; set; }

    public string sikayetTipi { get; set; }

    public int categoryId { get; set; }

    public int createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
