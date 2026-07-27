#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class MusteriSikayetKapatmaDto
{
    public int Id { get; set; }

    public string kapatmaAdi { get; set; }

    public int fabrikaId { get; set; }

    public int directorId { get; set; }

    public int createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
