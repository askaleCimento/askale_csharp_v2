#nullable enable
namespace AskalePortal.Data.RequestModels.Education;

public sealed class EducationSearchRequest
{
    public int? userId { get; set; }
    public int? sectionId { get; set; }
    public string? courseName { get; set; }
}

public sealed class EducationQuestionSectionSearchRequest
{
    public int? userId { get; set; }
    public int? sectionId { get; set; }
    public string? courseName { get; set; }
}

public sealed class EducationSectionSearchRequest
{
    public string? egitimBolumu { get; set; }
}
