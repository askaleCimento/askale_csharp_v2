#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ActionLogDto
{
    public int Id { get; set; }

    public int moduleId { get; set; }

    public int dataId { get; set; }

    public string actionType { get; set; }

    public int userId { get; set; }

    public string userName { get; set; }

    public string ip { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
