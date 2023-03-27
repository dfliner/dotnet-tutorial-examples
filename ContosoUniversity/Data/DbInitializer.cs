using ContosoUniversity.Models;

namespace ContosoUniversity.Data;

internal static class DbInitializer
{
    public static void Initialize(ContosoUniversityContext dbContext)
    {
        // dbContext.Database.EnsureCreated();

        if (dbContext.Students.Any())
        {
            return;
        }

        var students = new Student[]
        {
            new Student { FirstName = "Carson",   LastName = "Alexander", EnrollmentDate = DateTime.Parse("2010-09-01") },
            new Student { FirstName = "Meredith", LastName = "Alonso",    EnrollmentDate = DateTime.Parse("2012-09-01") },
            new Student { FirstName = "Arturo",   LastName = "Anand",     EnrollmentDate = DateTime.Parse("2013-09-01") },
            new Student { FirstName = "Gytis",    LastName = "Barzdukas", EnrollmentDate = DateTime.Parse("2012-09-01") },
            new Student { FirstName = "Yan",      LastName = "Li",        EnrollmentDate = DateTime.Parse("2012-09-01") },
            new Student { FirstName = "Peggy",    LastName = "Justice",   EnrollmentDate = DateTime.Parse("2011-09-01") },
            new Student { FirstName = "Laura",    LastName = "Norman",    EnrollmentDate = DateTime.Parse("2013-09-01") },
            new Student { FirstName = "Nino",     LastName = "Olivetto",  EnrollmentDate = DateTime.Parse("2005-09-01") }
        };
        dbContext.Students.AddRange(students);

        var officeAssignments = new OfficeAssignment[]
        {
            new OfficeAssignment { Location = "Smith 17" },
            new OfficeAssignment { Location = "Gowan 27" },
            new OfficeAssignment { Location = "Thompson 304" },
            new OfficeAssignment { Location = "North Building 102" }
        };
        dbContext.OfficeAssignments.AddRange(officeAssignments);

        var instructors = new Instructor[]
        {
            new Instructor { FirstName = "Kim",     LastName = "Abercrombie", HireDate = DateTime.Parse("1995-03-11"),
                OfficeAssignment = officeAssignments.Single(o => o.Location == "Smith 17") },
            new Instructor { FirstName = "Fadi",    LastName = "Fakhouri",    HireDate = DateTime.Parse("2002-07-06"),
                OfficeAssignment = officeAssignments.Single(o => o.Location == "Smith 17") },
            new Instructor { FirstName = "Roger",   LastName = "Harui",       HireDate = DateTime.Parse("1998-07-01"), },
            new Instructor { FirstName = "Candace", LastName = "Kapoor",      HireDate = DateTime.Parse("2001-01-15"),
                OfficeAssignment = officeAssignments.Single(o => o.Location == "Thompson 304") },
            new Instructor { FirstName = "Roger",   LastName = "Zheng",       HireDate = DateTime.Parse("2004-02-12"),
                OfficeAssignment = officeAssignments.Single(o => o.Location == "Gowan 27") },

        };
        dbContext.Instructors.AddRange(instructors);

        var departments = new Department[]
        {
            new Department { Name = "English",     Budget = 350000, StartDate = DateTime.Parse("2007-09-01"),
                Administrator  = instructors.Single( i => i.LastName == "Abercrombie") },
            new Department { Name = "Mathematics", Budget = 100000, StartDate = DateTime.Parse("2007-09-01"),
                Administrator  = instructors.Single( i => i.LastName == "Fakhouri") },
            new Department { Name = "Engineering", Budget = 350000, StartDate = DateTime.Parse("2007-09-01"),
                Administrator  = instructors.Single( i => i.LastName == "Harui") },
            new Department { Name = "Economics",   Budget = 100000, StartDate = DateTime.Parse("2007-09-01"),
                Administrator  = instructors.Single( i => i.LastName == "Kapoor") },
            new Department { Name = "Chemistry",   Budget = 120000, StartDate = DateTime.Parse("2010-06-30"),
                Administrator  = instructors.Single( i => i.LastName == "Harui") }
        };
        dbContext.Departments.AddRange(departments);

        var courses = new Course[]
        {
            new Course { CourseId = 1050, Title = "Chemistry",      Credit = 3,
                Department = departments.Single( s => s.Name == "Chemistry") },
            new Course { CourseId = 4022, Title = "Microeconomics", Credit = 3,
                Department = departments.Single( s => s.Name == "Economics") },
            new Course { CourseId = 4041, Title = "Macroeconomics", Credit = 3,
                Department = departments.Single( s => s.Name == "Economics") },
            new Course { CourseId = 1045, Title = "Calculus",       Credit = 4,
                Department = departments.Single( s => s.Name == "Mathematics") },
            new Course { CourseId = 3141, Title = "Trigonometry",   Credit = 4,
                Department = departments.Single( s => s.Name == "Mathematics") },
            new Course { CourseId = 2021, Title = "Composition",    Credit = 3,
                Department = departments.Single( s => s.Name == "English") },
            new Course { CourseId = 2042, Title = "Literature",     Credit = 4,
                Department = departments.Single( s => s.Name == "English") },
         };
        dbContext.Courses.AddRange(courses);

        var courseAssignments = new CourseAssignment[]
        {
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Chemistry" ),
                Instructor = instructors.Single(i => i.LastName == "Kapoor")
            },
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Chemistry" ),
                Instructor = instructors.Single(i => i.LastName == "Harui")
            },
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Microeconomics" ),
                Instructor = instructors.Single(i => i.LastName == "Zheng")
            },
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Macroeconomics" ),
                Instructor = instructors.Single(i => i.LastName == "Zheng")
            },
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Calculus" ),
                Instructor = instructors.Single(i => i.LastName == "Fakhouri")
            },
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Trigonometry" ),
                Instructor = instructors.Single(i => i.LastName == "Harui")
            },
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Composition" ),
                Instructor = instructors.Single(i => i.LastName == "Abercrombie")
            },
            new CourseAssignment {
                Course = courses.Single(c => c.Title == "Literature" ),
                Instructor = instructors.Single(i => i.LastName == "Abercrombie")
            },
        };
        dbContext.CoursesAssignments.AddRange(courseAssignments);

        var enrollments = new Enrollment[]
        {
            new Enrollment {
                Student = students.Single(s => s.LastName == "Alexander"),
                Course = courses.Single(c => c.Title == "Chemistry" ),
                Grade = Grade.A
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Alexander"),
                Course = courses.Single(c => c.Title == "Microeconomics" ),
                Grade = Grade.C
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Alexander"),
                Course = courses.Single(c => c.Title == "Macroeconomics" ),
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Alexander"),
                Course = courses.Single(c => c.Title == "Macroeconomics" ),
                Grade= Grade.A
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Alonso"),
                Course = courses.Single(c => c.Title == "Calculus" ),
                Grade = Grade.B
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Alonso"),
                Course = courses.Single(c => c.Title == "Trigonometry" ),
                Grade = Grade.B
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Alonso"),
                Course = courses.Single(c => c.Title == "Composition" ),
                Grade = Grade.B
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Anand"),
                Course = courses.Single(c => c.Title == "Chemistry" )
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Anand"),
                Course = courses.Single(c => c.Title == "Microeconomics"),
                Grade = Grade.B
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Barzdukas"),
                Course = courses.Single(c => c.Title == "Chemistry"),
                Grade = Grade.B
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Li"),
                Course = courses.Single(c => c.Title == "Composition"),
                Grade = Grade.B
            },
            new Enrollment {
                Student = students.Single(s => s.LastName == "Justice"),
                Course = courses.Single(c => c.Title == "Literature"),
                Grade = Grade.B
            }
        };
        dbContext.AddRange(enrollments);

        dbContext.SaveChanges();
    }
}
