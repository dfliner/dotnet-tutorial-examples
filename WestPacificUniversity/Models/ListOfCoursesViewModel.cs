using WestPacificUniversity.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WestPacificUniversity.Models;

public class ListOfCoursesViewModel
{
    public PaginatedList<Course>? Courses { get; set; }

    public SelectList? Departments { get; set; }
    public int? DepartmentId { get; set; }

    public string? CurrentFilter { get; set; }

    public string CurrentSort { get; set; } = "";
}
