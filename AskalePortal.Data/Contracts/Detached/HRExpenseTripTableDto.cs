#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRExpenseTripTableDto
{
    public int Id { get; set; }

    public int currentUserId { get; set; }

    public int currentStateId { get; set; }

    public int userId { get; set; }

    public int? vekaletId { get; set; }

    public DateTime? gidisTarihi { get; set; }

    public DateTime? donusTarihi { get; set; }

    public int destinationLocationId { get; set; }

    public string tripDescription { get; set; }

    public string digerDestination { get; set; }

    public int hereLocationId { get; set; }

    public int tripDescriptionId { get; set; }

    public decimal avans { get; set; }

    public int onaySirasi { get; set; }

    public DateTime createdDate { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public int createdUserId { get; set; }

    public bool? lastApproved { get; set; }

    public string disaprovecondition { get; set; }

    public bool? approval { get; set; }

    public bool enabled { get; set; }

}
