#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class RatingQuestionVoteDto
{
    public int Id { get; set; }

    public int userId { get; set; }

    public int ratingId { get; set; }

    public int questionId { get; set; }

    public int ratingValue { get; set; }

    public string comment { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public int? createdUserId { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
