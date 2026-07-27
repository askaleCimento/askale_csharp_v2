#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EgitimVideoIzlemeTableDto
{
    public int Id { get; set; }

    public int userId { get; set; }

    public int videoId { get; set; }

    public int videoSec { get; set; }

    public bool bittiMi { get; set; }

    public DateTime? izlemeTarihi { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
