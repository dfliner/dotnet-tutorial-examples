namespace WestPacificUniversity.Models;

public class CourseAssignment
{
    public int InstructorId { get; set; }
    public int CourseId { get; set; }

    public Instructor Instructor { get; set; } = default!;
    public Course Course { get; set; } = default!;
}
