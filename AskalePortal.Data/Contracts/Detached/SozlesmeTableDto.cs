#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class SozlesmeTableDto
{
    public int Id { get; set; }

    public int companyId { get; set; }

    public int sozlesmeTuruId { get; set; }

    public string satinAlmaGrubu { get; set; }

    public string firmaKodu { get; set; }

    public string iletisim { get; set; }

    public string firmaYetkilisi { get; set; }

    public string sozlesmeKonusu { get; set; }

    public string aciklama { get; set; }

    public decimal sozlesmeTutari { get; set; }

    public int sozlesmeTutarBirimiId { get; set; }

    public int sozlesmeOdemeVadesi { get; set; }

    public decimal? odemeAvansYuzdesi { get; set; }

    public decimal odemeAvansTutari { get; set; }

    public int odemeAvansBirimiId { get; set; }

    public string damgaVergisiOdemesi { get; set; }

    public DateTime imzalananTarih { get; set; }

    public DateTime baslangicTarihi { get; set; }

    public DateTime bitisTarihi { get; set; }

    public DateTime uyariTarihi { get; set; }

    public string bildirimYapilacakKisiler { get; set; }

    public bool teminatVarmi { get; set; }

    public DateTime? teminatBaslangic { get; set; }

    public DateTime? teminatBitis { get; set; }

    public decimal? teminatTutari { get; set; }

    public int? teminatTutariParaBirimId { get; set; }

    public bool tamamMi { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
