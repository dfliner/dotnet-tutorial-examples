using WestPacificUniversity.Data.Entities;

namespace WestPacificUniversity.Models;

public class ListOfInstructorsViewModel
{
    public IList<Instructor> Instructors { get; set; }
    public IList<Course> Courses { get; set; }
    public IList<Enrollment> Enrollments { get; set; }
}
