#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRExpenseAmountDto
{
    public int Id { get; set; }

    public int calisanTuruId { get; set; }

    public int harcamaTuruId { get; set; }

    public decimal harcirahMiktari { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public DateTime? gecerlilikTarihi { get; set; }

}
