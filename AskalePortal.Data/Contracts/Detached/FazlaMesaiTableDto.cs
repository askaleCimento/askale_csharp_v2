#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class FazlaMesaiTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string description { get; set; }

    public double gunlukSaat { get; set; }

    public double mesaiSaat { get; set; }

    public int? uniteId { get; set; }

}
