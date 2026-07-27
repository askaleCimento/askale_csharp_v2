#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class AnnualLeaveTypeDto
{
    public int Id { get; set; }

    public string typeName { get; set; }

    public string typeNameEn { get; set; }

    public string sapCode { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
