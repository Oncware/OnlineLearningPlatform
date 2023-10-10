public class StudentViewModel
{
    public string UserName { get; set; }
    public string UserId { get; set; }
    public decimal Process { get; set; }  // Calculated as (SubmittedAssignments/TotalAssignments)*100
    public IEnumerable<UserAssignment> UserAssignments { get; set; }
}
