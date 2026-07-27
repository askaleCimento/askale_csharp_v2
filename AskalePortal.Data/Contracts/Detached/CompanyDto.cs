#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class CompanyDto
{
    public int Id { get; set; }

    public string mandt { get; set; }

    public string spras { get; set; }

    public string vkorg { get; set; }

    public string vtext { get; set; }

    public string companySection { get; set; }

    public string imgUrl { get; set; }

    public string companyTitle { get; set; }

    public string companyLongName { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string companyShortName { get; set; }

    public int? companySectionId { get; set; }

}
