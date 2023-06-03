﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WestPacificUniversity.Data.Entities;

public class Department
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = default!;

    [DataType(DataType.Currency)]
    [Column(TypeName = "money")] // SQL data type for this column
    public decimal Budget { get; set; }

    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    public int? InstructorId { get; set; }

    // Name for nav property doesn't have pair with its FK?
    // By convention, it uses InstructorId as its FK
    public Instructor? Administrator { get; set; }

    [InverseProperty(nameof(Course.Department))]
    public ICollection<Course> Courses { get; set; } = default!;
}
