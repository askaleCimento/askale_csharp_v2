#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class AracTalepTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string aciklama { get; set; }

    public bool? approval { get; set; }

    public DateTime? baslangicTarihi { get; set; }

    public int? currentStateId { get; set; }

    public int? currentUserId { get; set; }

    public int? destinationLocationId { get; set; }

    public int? onaySirasi { get; set; }

    public DateTime? teslimTarihi { get; set; }

    public string plaka { get; set; }

}
