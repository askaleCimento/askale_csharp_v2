#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRMaasTablosuDto
{
    public int Id { get; set; }

    public string userName { get; set; }

    public string gorevi { get; set; }

    public string unvani { get; set; }

    public DateTime dogumTarihi { get; set; }

    public DateTime iseGirisTarihi { get; set; }

    public string lokasyon { get; set; }

    public string ucretTuru { get; set; }

    public decimal? maas { get; set; }

    public decimal? kira { get; set; }

    public decimal? yuksekLisans { get; set; }

    public decimal? yabanciDil { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
