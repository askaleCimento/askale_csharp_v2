#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EgitimSorulariTableDto
{
    public int Id { get; set; }

    public int videoId { get; set; }

    public TimeSpan showVideoTime { get; set; }

    public string soru { get; set; }

    public string sikA { get; set; }

    public string sikB { get; set; }

    public string sikC { get; set; }

    public string sikD { get; set; }

    public string sikE { get; set; }

    public string dogruCevap { get; set; }

    public bool sonSoruMu { get; set; }

    public int userId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public int? createdUserId { get; set; }

}
