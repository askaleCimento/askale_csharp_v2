#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class UserTelephoneTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string factoryInternal { get; set; }

    public string factoryNumber { get; set; }

    public string phoneNumber { get; set; }

    public string shortCode { get; set; }

    public int userId { get; set; }

    public bool? kvkkOnay { get; set; }

}
