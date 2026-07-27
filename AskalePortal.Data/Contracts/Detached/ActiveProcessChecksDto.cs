#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ActiveProcessChecksDto
{
    public int? id { get; set; }

    public DateTime? createdDate { get; set; }

    public DateTime? updateDate { get; set; }

    public int? createdUserId { get; set; }

    public int? updatedUserId { get; set; }

    public bool? enabled { get; set; }

    public string? belnr { get; set; }

    public string? kunnr { get; set; }

    public string? name1 { get; set; }

    public string? netdt { get; set; }

    public double? wrbtr { get; set; }

    public int? activeProcessId { get; set; }

}
