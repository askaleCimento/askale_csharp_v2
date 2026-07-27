#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class MusteriSikayetFormDto
{
    public int Id { get; set; }

    public int categoryId { get; set; }

    public string title { get; set; }

    public string musteriKodu { get; set; }

    public string musteriAdi { get; set; }

    public int companyId { get; set; }

    public int sikayetTipiId { get; set; }

    public string malzemeTuru { get; set; }

    public decimal? malzemeMiktari { get; set; }

    public int userId { get; set; }

    public string musteriTemsilcisi { get; set; }

    public string musteriTel { get; set; }

    public string musteriEmail { get; set; }

    public bool enabled { get; set; }

    public string description { get; set; }

    public DateTime createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
