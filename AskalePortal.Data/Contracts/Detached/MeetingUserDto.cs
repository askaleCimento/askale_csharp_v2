#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class MeetingUserDto
{
    public int Id { get; set; }

    public int dataOrder { get; set; }

    public string name { get; set; }

    public string title { get; set; }

    public string email { get; set; }

    public string mobile { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
