using Microsoft.EntityFrameworkCore;
using WestPacificUniversity.Data.Entities;

namespace WestPacificUniversity.Data;

public class WestPacificUniversityContext : DbContext
{
    public WestPacificUniversityContext (DbContextOptions<WestPacificUniversityContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; } = default!;
    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<Enrollment> Enrollments { get; set; } = default!;

    public DbSet<Instructor> Instructors { get; set; } = default!;
    public DbSet<Department> Departments { get; set; } = default!;
    public DbSet<CourseAssignment> CoursesAssignments { get; set; } = default!;
    public DbSet<OfficeAssignment> OfficeAssignments { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Another way to set table names
        modelBuilder.Entity<Instructor>().ToTable("Instructors");
        modelBuilder.Entity<Department>().ToTable("Departments");
        modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");

        // Set composite primay key
        modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment")
            .HasKey(cs => new {cs.CourseId, cs.InstructorId});

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Department)
            .WithMany(d => d.Courses)
            .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<Enrollment>()
        //    .HasIndex(e => new { e.StudentId, e.CourseId });
    }
}
