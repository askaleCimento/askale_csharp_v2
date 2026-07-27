#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HelpDeskDemandDto
{
    public int Id { get; set; }

    public string ticketNumber { get; set; }

    public int helpDeskTypeId { get; set; }

    public int helpDeskStatusId { get; set; }

    public int helpDeskCategoryId { get; set; }

    public int createdUserId { get; set; }

    public int createdByCompanyId { get; set; }

    public string createdByUserName { get; set; }

    public string notificationMails { get; set; }

    public int? assignedToHelpDeskRoleId { get; set; }

    public string internalNumber { get; set; }

    public string teamviewerId { get; set; }

    public string teamviewerPassword { get; set; }

    public string title { get; set; }

    public string description { get; set; }

    public string timeSpent { get; set; }

    public DateTime createdDate { get; set; }

    public bool isClosed { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
