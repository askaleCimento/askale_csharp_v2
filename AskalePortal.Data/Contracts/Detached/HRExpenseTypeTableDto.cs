#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRExpenseTypeTableDto
{
    public int Id { get; set; }

    public string expenseTypeName { get; set; }

    public bool toplamaNo { get; set; }

    public bool harcamaBoyu { get; set; }

    public bool otoparkMi { get; set; }

    public string sapSide { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
