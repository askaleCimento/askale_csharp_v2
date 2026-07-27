#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class IncomingDocumentSourceDto
{
    public int Id { get; set; }

    public string title { get; set; }

    public string subTitle { get; set; }

    public string subject { get; set; }

    public string phone { get; set; }

    public string fax { get; set; }

    public bool enabled { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
