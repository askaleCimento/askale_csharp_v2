#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ParaBirimiTableDto
{
    public int Id { get; set; }

    public string paraBirimi { get; set; }

    public string paraDuzeni { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
