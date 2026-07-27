#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EgitimTableDto
{
    public int Id { get; set; }

    public int egitimBolumId { get; set; }

    public string courseName { get; set; }

    public DateTime? startDate { get; set; }

    public DateTime? endDate { get; set; }

    public int createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public int sira { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
