#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class EducationQuestionSectionTableDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public int? courseId { get; set; }

    public string questionName { get; set; }

    public int? questionOrder { get; set; }

    public string questionPicturePath { get; set; }

}
