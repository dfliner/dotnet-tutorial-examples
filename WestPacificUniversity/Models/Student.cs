using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WestPacificUniversity.Models;

/// <summary>
/// In real-world app, this should be an entity class, not directly used outside the data layer.
/// But in this turorial, we use it directly in MVC views for simlicity purpose.
/// <para>
/// See <see href="https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types">
/// nullbitily in EF Core</see>.
/// </para>
/// </summary>
[Table("Students")]
public class Student
{
    public int Id { get; set; } // StudentId

    [Display(Name = "Last Name")]
    [Required, MaxLength(50)]// maps to column data type nvarchar(50)
    public string LastName { get; set; } = default!;

    [Display(Name = "First Name")]
    [RegularExpression(@"^[A-Z]+[a-zA-Z]*$")] // validation rule
    [Required, StringLength(50, MinimumLength = 3)] // MinLength has no effect on database schema
    public string FirstName { get; set; } = default!;

    [Display(Name = "Enrollment Date")]
    [DataType(DataType.Date)] // Does not impact data type in database.
    public DateTime EnrollmentDate { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => string.Join(", ", LastName, FirstName);

    /// <summary>
    /// Student may enroll in multiple courses, and vice versa.
    /// </summary>
    public ICollection<Enrollment>? Enrollments { get; set; }
}
