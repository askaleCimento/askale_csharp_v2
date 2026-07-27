#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ActiveProcessDto
{
    public int Id { get; set; }

    public int approvalProcessId { get; set; }

    public int currentStateId { get; set; }

    public int currentUserId { get; set; }

    public int? userVekaletId { get; set; }

    public string dagitimKanali { get; set; }

    public string relatedData { get; set; }

    public string relatedDataId { get; set; }

    public string relatedDataDesc { get; set; }

    public string relatedDataPrimary { get; set; }

    public string relatedDataPrimaryId { get; set; }

    public string relatedDataPrimaryDesc { get; set; }

    public string relatedColumn { get; set; }

    public string dataType { get; set; }

    public string oldValue { get; set; }

    public string newValue { get; set; }

    public string description { get; set; }

    public string customFields { get; set; }

    public int createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public string disaprovecondition { get; set; }

    public int? oncekiArtirim { get; set; }

    public string? belgeTutari { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public double avg_days { get; set; }

    public double avg_vade { get; set; }

}
