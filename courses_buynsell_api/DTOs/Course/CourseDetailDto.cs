namespace courses_buynsell_api.DTOs.Course;

public class CourseDetailDto : CourseListItemDto
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsApproved { get; set; }
}