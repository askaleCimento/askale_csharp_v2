#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class AnnualLeaveTableDto
{
    public int Id { get; set; }

    public int currentStateId { get; set; }

    public int currentUserId { get; set; }

    public int userId { get; set; }

    public DateTime enteredDate { get; set; }

    public string departmanName { get; set; }

    public string job { get; set; }

    public int typeId { get; set; }

    public decimal dayleft { get; set; }

    public decimal dayRequested { get; set; }

    public DateTime startDate { get; set; }

    public DateTime endDate { get; set; }

    public string adress { get; set; }

    public int? vekaletId { get; set; }

    public int siraNo { get; set; }

    public string disaprovecondition { get; set; }

    public string digerAciklama { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
