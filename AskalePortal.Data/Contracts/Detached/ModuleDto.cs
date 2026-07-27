#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ModuleDto
{
    public int Id { get; set; }

    public string title { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
