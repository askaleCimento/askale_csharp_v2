#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ConfigDto
{
    public int Id { get; set; }

    public string title { get; set; }

    public string name { get; set; }

    public string appServerHost { get; set; }

    public string systemNumber { get; set; }

    public string userName { get; set; }

    public string password { get; set; }

    public string client { get; set; }

    public string language { get; set; }

    public string poolSize { get; set; }

    public string peakConnectionsLimit { get; set; }

    public string idleTimeout { get; set; }

    public string systemId { get; set; }

    public string sapRouter { get; set; }

    public bool isDefault { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
