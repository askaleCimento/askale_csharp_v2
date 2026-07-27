#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ToplantiKararlariDto
{
    public int Id { get; set; }

    public int KararNo { get; set; }

    public int? companyId { get; set; }

    public int toplantiNo { get; set; }

    public DateTime? toplantiTarihi { get; set; }

    public int toplantiYeriId { get; set; }

    public string toplantiKonusu { get; set; }

    public string katilimcilar { get; set; }

    public int? sorumluDepartmanId { get; set; }

    public string yapilacakIs { get; set; }

    public string isdurumu { get; set; }

    public string baslangicSuresi { get; set; }

    public string bitisSuresi { get; set; }

    public string durumu { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
