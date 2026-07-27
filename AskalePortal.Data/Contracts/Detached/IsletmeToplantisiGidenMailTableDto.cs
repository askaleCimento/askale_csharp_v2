#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class IsletmeToplantisiGidenMailTableDto
{
    public int Id { get; set; }

    public int? companyId { get; set; }

    public int? mailUserId { get; set; }

    public string gitmeTuru { get; set; }

    public bool? enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
