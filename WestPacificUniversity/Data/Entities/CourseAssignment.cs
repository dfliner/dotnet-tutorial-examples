namespace WestPacificUniversity.Data.Entities;

public class CourseAssignment
{
    // Composite primary key is specified in context

    public int InstructorId { get; set; }
    public int CourseId { get; set; }

    public Instructor Instructor { get; set; } = default!;
    public Course Course { get; set; } = default!;
}
