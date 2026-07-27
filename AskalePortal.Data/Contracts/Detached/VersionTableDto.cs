#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class VersionTableDto
{
    public int Id { get; set; }

    public int type { get; set; }

    public required string currentVersion { get; set; }

    public int platform { get; set; }

    public bool enabled { get; set; }

}
