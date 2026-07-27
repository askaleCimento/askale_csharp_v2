#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ISGAksiyonTableDto
{
    public int Id { get; set; }

    public int companyId { get; set; }

    public int uygunsuzlukKaynagiId { get; set; }

    public DateTime? uygunsuzlukTarihi { get; set; }

    public int uygunsuzlukBulunanUniteId { get; set; }

    public int bidirimdeBulunan { get; set; }

    public string uygunsuzlukAciklama { get; set; }

    public string uygunsuzlukOneri { get; set; }

    public string fileOnceki { get; set; }

    public string fileSonraki { get; set; }

    public bool bittiMi { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
