#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class DahiliYazismalarDetayTableDto
{
    public int Id { get; set; }

    public int dahiliYazismaId { get; set; }

    public int userId { get; set; }

    public int? vekaletId { get; set; }

    public bool? approved { get; set; }

    public DateTime createdDate { get; set; }

    public bool isReplied { get; set; }

    public DateTime? replyDate { get; set; }

    public bool kanalBittiMi { get; set; }

    public bool sonOnayMi { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
