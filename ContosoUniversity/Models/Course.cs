using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

[Table("Courses")]
public class Course
{
    /// <summary>
    /// Each course has its own assigned course number.
    /// </summary>
    [Display(Name = "Course Number")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CourseId { get; set; }

    [Display(Name = "Course Name")]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = default!;

    [Range(0, 5)]
    public int Credit { get; set; }

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public ICollection<Enrollment>? Enrollments { get; set; }
    public ICollection<CourseAssignment>? CourseAssignments { get; set; }
}
