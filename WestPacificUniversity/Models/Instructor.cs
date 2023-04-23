using System.ComponentModel.DataAnnotations;

namespace WestPacificUniversity.Models;

public class Instructor
{
    public int Id { get; set; }

    [Display(Name = "Last Name")]
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = default!;

    [Display(Name = "First Name")]
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = default!;

    [Display(Name = "Hire Date")]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => LastName + ", " + FirstName;

    // Pros and cons for not using an explicit FK?
    public OfficeAssignment? OfficeAssignment { get; set; }

    public ICollection<CourseAssignment>? CourseAssignments { get; set; }
}
