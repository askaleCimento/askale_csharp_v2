#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class VekaletDto
{
    public int Id { get; set; }

    public int verenId { get; set; }

    public int alanId { get; set; }

    public DateTime? tarih { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
