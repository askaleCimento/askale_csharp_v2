#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class TiklamaSayaciTableDto
{
    public int Id { get; set; }

    public int userId { get; set; }

    public DateTime createdDate { get; set; }

    public string neresi { get; set; }

    public int sayac { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
