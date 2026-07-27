#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRVergiDilimleriTableDto
{
    public int Id { get; set; }

    public int yil { get; set; }

    public decimal? vergiDilimi5 { get; set; }

    public decimal matrahSiniri5 { get; set; }

    public decimal? vergiDilimi4 { get; set; }

    public decimal matrahSiniri4 { get; set; }

    public decimal? vergiDilimi3 { get; set; }

    public decimal matrahSiniri3 { get; set; }

    public decimal? vergiDilimi2 { get; set; }

    public decimal matrahSiniri2 { get; set; }

    public decimal? vergiDilimi1 { get; set; }

    public decimal matrahSiniri1 { get; set; }

    public decimal? sgkTavan { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
