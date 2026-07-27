#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class RaporTipleriDto
{
    public int Id { get; set; }

    public string raporAdi { get; set; }

    public string raporTipi { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
