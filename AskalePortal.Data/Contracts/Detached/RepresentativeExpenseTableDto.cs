#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class RepresentativeExpenseTableDto
{
    public int Id { get; set; }

    public int userId { get; set; }

    public int currentUserId { get; set; }

    public int currentStateId { get; set; }

    public DateTime? spendingTime { get; set; }

    public int typeId { get; set; }

    public string description { get; set; }

    public decimal amount { get; set; }

    public decimal approvedAmount { get; set; }

    public bool approval { get; set; }

    public int onaySirasi { get; set; }

    public string fileNames { get; set; }

    public string disaproveCondition { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
