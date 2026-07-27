#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class LoginLogDto
{
    public int Id { get; set; }

    public string username { get; set; }

    public string password { get; set; }

    public bool isSuccess { get; set; }

    public string iP { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
