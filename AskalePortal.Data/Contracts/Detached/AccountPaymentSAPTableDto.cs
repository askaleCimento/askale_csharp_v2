#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class AccountPaymentSAPTableDto
{
    public string oenum { get; set; }

    public string cpudt { get; set; }

    public string cputm { get; set; }

    public string usnam { get; set; }

    public string aedat { get; set; }

    public string aeuhr { get; set; }

    public string aenam { get; set; }

    public string bukrs { get; set; }

    public string hkont { get; set; }

    public string kurumKodu { get; set; }

    public string SubeKodu { get; set; }

    public string name1 { get; set; }

    public string unva1 { get; set; }

    public string name2 { get; set; }

    public string unva2 { get; set; }

    public string bstat { get; set; }

    public string belnr { get; set; }

    public string gjahr { get; set; }

    public string zsayino { get; set; }

    public string znot { get; set; }

    public string bankl { get; set; }

    public string bankn { get; set; }

    public string iban { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public DateTime? updateDate { get; set; }

    public int? createdUserId { get; set; }

    public int? updatedUserId { get; set; }

}
