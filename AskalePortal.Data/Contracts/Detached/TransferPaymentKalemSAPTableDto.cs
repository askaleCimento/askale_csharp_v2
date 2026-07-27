#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class TransferPaymentKalemSAPTableDto
{
    public int Id { get; set; }

    public int currentStateId { get; set; }

    public int currentUserId { get; set; }

    public string henum { get; set; }

    public string posnr { get; set; }

    public string firma { get; set; }

    public string lifnr { get; set; }

    public string banka { get; set; }

    public string bankl { get; set; }

    public string brnch { get; set; }

    public string bankn { get; set; }

    public string iban { get; set; }

    public string wrbtr { get; set; }

    public string waerk { get; set; }

    public string aciklama { get; set; }

    public bool? approval { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public int? transferPaymentSAP { get; set; }

}
