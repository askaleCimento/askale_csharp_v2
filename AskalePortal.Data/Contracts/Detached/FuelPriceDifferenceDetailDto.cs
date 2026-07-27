#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class FuelPriceDifferenceDetailDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public bool? approved { get; set; }

    public int? fuelId { get; set; }

    public Guid guid { get; set; }

    public bool? isReplied { get; set; }

    public DateTime? replyDate { get; set; }

    public int? userId { get; set; }

}
