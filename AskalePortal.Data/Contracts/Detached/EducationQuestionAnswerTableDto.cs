#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EducationQuestionAnswerTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string cevap { get; set; }

    public int? soruId { get; set; }

    public int? userId { get; set; }

}
