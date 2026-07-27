#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HREmployeeTypeDto
{
    public int Id { get; set; }

    public string calisanTuru { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
