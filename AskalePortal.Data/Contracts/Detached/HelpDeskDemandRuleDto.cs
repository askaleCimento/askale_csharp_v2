#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HelpDeskDemandRuleDto
{
    public int Id { get; set; }

    public string companies { get; set; }

    public string helpDeskCategories { get; set; }

    public int helpDeskRoleId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
