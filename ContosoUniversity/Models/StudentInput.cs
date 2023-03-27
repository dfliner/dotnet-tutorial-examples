namespace ContosoUniversity.Models;

/// <summary>
/// DTO class for creating/editing <see cref="Student"/> entity.
/// </summary>
public class StudentInput
{
    public int Id { get; set; }
    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public DateTime EnrollmentDate { get; set; }
}
