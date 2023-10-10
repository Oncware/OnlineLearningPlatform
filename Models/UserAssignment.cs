using JustLearn1.Models;

public class UserAssignment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int AssignmentId { get; set; }
    public bool IsSubmitted { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public int? Score { get; set; }
    // ... Diğer gereken özellikler ...
    public string? UserAnswer { get; set; }
    public ApplicationUser User { get; set; }
    public Assignment Assignment { get; set; }
}
