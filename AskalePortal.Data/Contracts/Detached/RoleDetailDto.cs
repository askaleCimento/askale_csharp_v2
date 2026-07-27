#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class RoleDetailDto
{
    public int Id { get; set; }

    public int roleId { get; set; }

    public int moduleId { get; set; }

    public bool canSee { get; set; }

    public bool canAdd { get; set; }

    public bool canEdit { get; set; }

    public bool canDelete { get; set; }

    public bool canApprove { get; set; }

    public bool canSeeLogs { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
