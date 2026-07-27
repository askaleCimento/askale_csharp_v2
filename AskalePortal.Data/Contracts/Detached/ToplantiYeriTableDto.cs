#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ToplantiYeriTableDto
{
    public int Id { get; set; }

    public int? companyId { get; set; }

    public string toplantiYeri { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
