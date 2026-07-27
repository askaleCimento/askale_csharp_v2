#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class SevkiyatPlakaDto
{
    public int Id { get; set; }

    public string plakaNo { get; set; }

    public string urunTipi { get; set; }

    public string urunCinsi { get; set; }

    public DateTime? aracCikis { get; set; }

    public bool listedeMi { get; set; }

    public int createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? sortOrder { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
