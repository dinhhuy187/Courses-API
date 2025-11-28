namespace courses_buynsell_api.DTOs.Course;

public class CourseStudentDto
{
    public string StudentName { get; set; } = string.Empty;
    public decimal PurchasedAmount { get; set; }
    public DateTime EnrollAt { get; set; }
}