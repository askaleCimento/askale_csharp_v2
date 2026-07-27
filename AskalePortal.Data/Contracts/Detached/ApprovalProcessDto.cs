#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ApprovalProcessDto
{
    public int Id { get; set; }

    public int companyId { get; set; }

    public int typeId { get; set; }

    public string dagitimKanali { get; set; }

    public string description { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
