using JustLearn1.Models;

public class Assignment
{
    public int AssignmentID { get; set; }
    public int ProductID { get; set; }
    public string Name { get; set; }
    public string Detail { get; set; }
    public DateTime DueDate { get; set; }
    public Product? Product { get; set; }

}
