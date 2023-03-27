using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

/// <summary>
/// In real-world app, this should be an entity class, not directly used outside the data layer.
/// But in this turorial, we use it directly in MVC views for simlicity purpose.
/// <para></para>
/// See <see href="https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types">nullbitily in EF Core</see>.
/// </summary>
[Table("Students")]
public class Student
{
    public int Id { get; set; }

    [Display(Name = "Last Name")]
    [Required]
    [MaxLength(50)] // maps to column data type nvarchar(50)
    public string LastName { get; set; } = default!;

    [Display(Name = "First Name")]
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = default!;


    [Display(Name = "Enrollment Date")]
    [DataType(DataType.Date)]
    public DateTime EnrollmentDate { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => LastName + ", " + FirstName;

    public ICollection<Enrollment>? Enrollments { get; set; }
}
