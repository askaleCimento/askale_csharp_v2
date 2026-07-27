#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class DahiliYazismaMessageDto
{
    public int Id { get; set; }

    public int dahiliYazismaId { get; set; }

    public int userId { get; set; }

    public int? sendUserId { get; set; }

    public string message { get; set; }

    public DateTime createdDate { get; set; }

    public bool showAll { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
