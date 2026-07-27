#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRVekaletTableDto
{
    public int Id { get; set; }

    public int vekaletVerenId { get; set; }

    public int vekaletAlanId { get; set; }

    public DateTime? tarih { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
