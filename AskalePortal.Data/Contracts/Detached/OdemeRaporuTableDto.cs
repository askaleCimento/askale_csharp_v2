#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class OdemeRaporuTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string balnr { get; set; }

    public string blart { get; set; }

    public string budat { get; set; }

    public string bukrs { get; set; }

    public string buzei { get; set; }

    public decimal? dmbe2 { get; set; }

    public decimal? dmbe3 { get; set; }

    public decimal? dmbtr { get; set; }

    public string gjahr { get; set; }

    public string hkont { get; set; }

    public string hwae2 { get; set; }

    public string hwae3 { get; set; }

    public string hwaer { get; set; }

    public string kunnr { get; set; }

    public string lifnr { get; set; }

    public string name1 { get; set; }

    public string sgtxt { get; set; }

    public string shkzg { get; set; }

    public string waers { get; set; }

    public decimal? wrbtr { get; set; }

    public string zuonr { get; set; }

}
