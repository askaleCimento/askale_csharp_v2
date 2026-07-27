#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ApprovalProcessDetailDto
{
    public int Id { get; set; }

    public int processId { get; set; }

    public int userId { get; set; }

    public decimal deger { get; set; }

    public int? vekaletId { get; set; }

    public int dataOrder { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
