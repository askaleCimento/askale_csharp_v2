#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EgitimVideoTableDto
{
    public int Id { get; set; }

    public string videoName { get; set; }

    public int courseId { get; set; }

    public string videoPath { get; set; }

    public int videoOrder { get; set; }

    public int createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string imagePath { get; set; }

}
