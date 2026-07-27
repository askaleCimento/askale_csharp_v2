#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class SatisOzetDto
{
    public int Id { get; set; }

    public DateTime? tarih { get; set; }

    public string satorg { get; set; }

    public decimal? oyilGun { get; set; }

    public decimal? cyilGun { get; set; }

    public decimal? oayGun { get; set; }

    public decimal? cayGun { get; set; }

    public decimal? oYil { get; set; }

    public decimal? cYil { get; set; }

    public string raporTipi { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string satorgAdi { get; set; }

}
