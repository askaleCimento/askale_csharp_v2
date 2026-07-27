#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class DieselPriceDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public bool? approval { get; set; }

    public int? companyId { get; set; }

    public int? currentStateId { get; set; }

    public int? currentUserId { get; set; }

    public decimal fiyat { get; set; }

    public DateTime girisTarihi { get; set; }

    public int kdvRate { get; set; }

    public int? onaySirasi { get; set; }

}
