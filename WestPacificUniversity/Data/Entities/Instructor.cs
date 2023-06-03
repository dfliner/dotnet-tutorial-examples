using System.ComponentModel.DataAnnotations;

namespace WestPacificUniversity.Data.Entities;

public class Instructor
{
    public int Id { get; set; }

    [Display(Name = "Last Name")]
    [Required, StringLength(50)]
    public string LastName { get; set; } = default!;

    [Display(Name = "First Name")]
    [Required, StringLength(50)]
    public string FirstName { get; set; } = default!;

    [Display(Name = "Hire Date")]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => LastName + ", " + FirstName;

    // Pros and cons for not using an explicit FK?
    public OfficeAssignment? OfficeAssignment { get; set; }

    /// <summary>
    /// Many-to-Many relationship can also mapped by convention, which by default create the join table transparently.
    /// <para>
    /// The <see cref="CourseAssignment"/> isn't needed if we go by convention.
    /// Refer to <seealso href="https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many"/>. 
    /// </para>
    /// </summary>
    public ICollection<CourseAssignment>? CourseAssignments { get; set; }
}
