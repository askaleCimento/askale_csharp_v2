#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class UserFormDto
{
    public int Id { get; set; }

    public int userFormTypeId { get; set; }

    public int userId { get; set; }

    public string formDataTemplate { get; set; }

    public string formDataForReport { get; set; }

    public DateTime createdDate { get; set; }

    public string term { get; set; }

    public bool isOK { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
