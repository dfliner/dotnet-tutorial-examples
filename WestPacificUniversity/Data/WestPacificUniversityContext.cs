using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using WestPacificUniversity.Data.Entities;
using WestPacificUniversity.EFCore.Entities;

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

        ConfigureFilters(modelBuilder);
    }

    protected virtual void ConfigureFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureFiltersMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(null, new object[] { modelBuilder, entityType! });
        }
    }

    private static MethodInfo ConfigureFiltersMethodInfo => GetGenericConfigureFiltersMethodInfo()!;

    private static MethodInfo? GetGenericConfigureFiltersMethodInfo()
    {
        return
            typeof(WestPacificUniversityContext).GetMethod(
                nameof(ConfigureFilters),
                1,
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(ModelBuilder), typeof(IMutableEntityType) },
                null
            );
    }

    private static void ConfigureFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
        where TEntity : class
    {
        Debug.Assert(typeof(TEntity) == entityType.ClrType);

        Expression<Func<TEntity, bool>>? filterExpression = null;
        if (typeof(TEntity).IsAssignableTo(typeof(ISoftDelete)))
        {
            modelBuilder.Entity<TEntity>()
                .Property(e => ((ISoftDelete)e).IsDeleted)
                .HasDefaultValue(false);

            filterExpression = entity => !((ISoftDelete)entity).IsDeleted;
        }

        if (filterExpression != null)
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
        }
    }
}
