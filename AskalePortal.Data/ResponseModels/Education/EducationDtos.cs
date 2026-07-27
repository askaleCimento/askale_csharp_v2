#nullable enable
namespace AskalePortal.Data.ResponseModels.Education;

public sealed class EducationDto
{
    public int id { get; set; }
    public string? sectionName { get; set; }
    public string? courseName { get; set; }
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }
    public DateTime? olusturmaTarihi { get; set; }
    public string? olusturanKisi { get; set; }
    public List<EducationVideoDto> listEducationVideoDto { get; set; } = [];
}
public sealed class EducationVideoDto
{
    public int id { get; set; }
    public string? videoName { get; set; }
    public int? videoOrder { get; set; }
    public DateTime? olusturmaTarihi { get; set; }
    public string? olusturanKisi { get; set; }
    public string? fileName { get; set; }
    public List<EgitimSorulariDto> listEducationQuestionDto { get; set; } = [];
}
public sealed class EgitimSorulariDto
{
    public int id { get; set; }
    public string? soru { get; set; }
    public string? sikA { get; set; }
    public string? sikB { get; set; }
    public string? sikC { get; set; }
    public string? sikD { get; set; }
    public string? sikE { get; set; }
    public string? dogruCevap { get; set; }
    public TimeSpan? showVideoTime { get; set; }
    public DateTime? olusturmaTarihi { get; set; }
    public string? olusturanKisi { get; set; }
}
public sealed class EducationQuestionSectionDto
{
    public int id { get; set; }
    public string? sectionName { get; set; }
    public string? questionName { get; set; }
    public DateTime? olusturmaTarihi { get; set; }
    public string? olusturanKisi { get; set; }
    public List<EducationQuestionDto> listEducationQuestionDto { get; set; } = [];
}
public sealed class EducationQuestionDto
{
    public int id { get; set; }
    public string? soru { get; set; }
    public string? sikA { get; set; }
    public string? sikB { get; set; }
    public string? sikC { get; set; }
    public string? sikD { get; set; }
    public string? sikE { get; set; }
    public string? dogruCevap { get; set; }
    public int? questionOrder { get; set; }
    public DateTime? olusturmaTarihi { get; set; }
    public string? olusturanKisi { get; set; }
}
