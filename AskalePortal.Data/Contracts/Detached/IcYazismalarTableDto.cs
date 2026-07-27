#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class IcYazismalarTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string backNote { get; set; }

    public string backNoteKanal { get; set; }

    public string backNoteMudur { get; set; }

    public bool? bilgiBittiMi { get; set; }

    public int? bilgiUserId1 { get; set; }

    public int? bilgiUserId2 { get; set; }

    public int? bilgiUserId3 { get; set; }

    public int? bilgiUserId4 { get; set; }

    public int? bilgiUserId5 { get; set; }

    public int? birimAmiriId { get; set; }

    public bool? bittiMi { get; set; }

    public int? companyId { get; set; }

    public string disaprovecondition { get; set; }

    public string icerik { get; set; }

    public string kanalGorusu { get; set; }

    public string kanalGorusuAsil { get; set; }

    public string kanalGorusuCeo { get; set; }

    public string kanalGorusuFirst { get; set; }

    public bool? kanalGorusuOkmi { get; set; }

    public int? kanalGorusuUserId { get; set; }

    public int? kanalId { get; set; }

    public string konu { get; set; }

    public string lastNote { get; set; }

    public int? lastUserId { get; set; }

    public int? lastUserId2 { get; set; }

    public int? onay1Kanal { get; set; }

    public bool? onay1Ok { get; set; }

    public bool? onay2Ok { get; set; }

    public bool? onay3Ok { get; set; }

    public bool? onay4Ok { get; set; }

    public bool? onaylandiMi { get; set; }

    public int? onaylayici1 { get; set; }

    public int? onaylayici2 { get; set; }

    public int? onaylayici3 { get; set; }

    public int? onaylayici4 { get; set; }

    public bool? redEttiMi { get; set; }

    public string servisi { get; set; }

    public DateTime? tarih { get; set; }

}
