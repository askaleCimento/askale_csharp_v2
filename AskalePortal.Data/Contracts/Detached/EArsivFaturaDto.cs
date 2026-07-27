#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EArsivFaturaDto
{
    public string ettn { get; set; }

    public string belgeNumarasi { get; set; }

    public DateTime? belgeTarihi { get; set; }

    public string belgeTuru { get; set; }

    public bool bittiMi { get; set; }

    public int? companyId { get; set; }

    public bool? enabled { get; set; }

    public int? iptalItiraz { get; set; }

    public string onayDurumu { get; set; }

    public string saticiUnvanAdSoyad { get; set; }

    public string saticiVknTckn { get; set; }

    public int? talepDurum { get; set; }

    public int? userId { get; set; }

    public DateTime? pullTime { get; set; }

}
