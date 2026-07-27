#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class MenuDto
{
    public int Id { get; set; }

    public int locationId { get; set; }

    public int topId { get; set; }

    public int dataOrder { get; set; }

    public string title { get; set; }

    public string link { get; set; }

    public string imageUrl { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
