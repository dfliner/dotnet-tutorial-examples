using WestPacificUniversity.Data.Entities;

namespace WestPacificUniversity.Models;

public class ListOfInstructorsViewModel
{
    public IList<Instructor> Instructors { get; set; } = default!;

    /// <summary>
    /// List of courses taught by the selected instructor.
    /// </summary>
    public IList<Course>? Courses { get; set; }

    /// <summary>
    /// List of students enrolled in the selected course.
    /// </summary>
    public IList<Enrollment>? Enrollments { get; set; }
}
