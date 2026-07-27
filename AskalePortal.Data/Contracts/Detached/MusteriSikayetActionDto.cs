#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class MusteriSikayetActionDto
{
    public int Id { get; set; }

    public int sikayetId { get; set; }

    public int aksiyonTipiId { get; set; }

    public string actionDescription { get; set; }

    public DateTime? actionDate { get; set; }

    public int companyId { get; set; }

    public int createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
