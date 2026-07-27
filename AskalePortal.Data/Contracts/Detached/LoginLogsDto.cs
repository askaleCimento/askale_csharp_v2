#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class LoginLogsDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string iP { get; set; }

    public bool isSuccess { get; set; }

    public string username { get; set; }

}
