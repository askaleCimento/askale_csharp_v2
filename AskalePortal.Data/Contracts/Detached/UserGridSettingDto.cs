#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class UserGridSettingDto
{
    public int Id { get; set; }

    public int userId { get; set; }

    public string pageName { get; set; }

    public bool isMobile { get; set; }

    public string gridSettings { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
