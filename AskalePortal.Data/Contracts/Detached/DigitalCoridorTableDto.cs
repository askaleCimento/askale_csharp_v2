#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class DigitalCoridorTableDto
{
    public int Id { get; set; }

    public string username { get; set; }

    public string location { get; set; }

    public int sectionId { get; set; }

    public string email { get; set; }

    public string user1 { get; set; }

    public string user2 { get; set; }

    public string user3 { get; set; }

    public string user4 { get; set; }

    public string user5 { get; set; }

    public string user6 { get; set; }

    public string user7 { get; set; }

    public string user8 { get; set; }

    public string user9 { get; set; }

    public string user10 { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
