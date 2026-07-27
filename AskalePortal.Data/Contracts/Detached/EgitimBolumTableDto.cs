#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EgitimBolumTableDto
{
    public int Id { get; set; }

    public int fabrikaId { get; set; }

    public string egitimBolumu { get; set; }

    public int sira { get; set; }

    public int createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
