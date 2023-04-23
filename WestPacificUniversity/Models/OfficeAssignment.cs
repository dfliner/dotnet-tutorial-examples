using System.ComponentModel.DataAnnotations;

namespace WestPacificUniversity.Models;

public class OfficeAssignment
{
    public int Id { get; set; }

    [Display(Name = "Office")]
    [Required]
    [StringLength(25)]
    public string Location { get; set; } = default!;

    public ICollection<Instructor>? Instructors { get; set; }
}
