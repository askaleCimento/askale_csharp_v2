#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class SaticiFirmalarTableDto
{
    public int Id { get; set; }

    public int companyId { get; set; }

    public string firmaKodu { get; set; }

    public string firmaAdi { get; set; }

    public string iletisim { get; set; }

    public string yetkilisi { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
