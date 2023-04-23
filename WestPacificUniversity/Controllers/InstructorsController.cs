using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestPacificUniversity.Data;
using WestPacificUniversity.Models;

namespace WestPacificUniversity.Controllers;

public class InstructorsController : Controller
{
    private readonly WestPacificUniversityContext _context;

    public InstructorsController(WestPacificUniversityContext context)
    {
        _context = context;
    }

    // GET: Instructors
    public async Task<IActionResult> Index(int? id, int? courseId)
    {
        var viewModel = new ListOfInstructorsViewModel();
        viewModel.Instructors = await _context.Instructors
            .Include(i => i.OfficeAssignment)
            .Include(i => i.CourseAssignments)
                .ThenInclude(c => c.Course)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
            .Include(i => i.CourseAssignments)
                .ThenInclude(c => c.Course)
                    .ThenInclude(c => c.Department)
            .AsNoTracking()
            .OrderBy(i => i.LastName)
            .ToListAsync();

        if (id != null)
        {
            ViewData["InstructorId"] = id.Value;
            Instructor instructor = viewModel.Instructors.Where(
                i => i.Id == id.Value).Single();
            viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course).ToList();
        }

        if (courseId != null)
        {
            ViewData["CourseId"] = courseId.Value;
            viewModel.Enrollments = viewModel.Courses.Where(
                x => x.CourseId == courseId.Value).Single().Enrollments.ToList();
        }

          return View(viewModel);
    }

    // GET: Instructors/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Instructors == null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(m => m.Id == id);
        if (instructor == null)
        {
            return NotFound();
        }

        return View(instructor);
    }

    // GET: Instructors/Create
    public IActionResult Create()
    {
        var instructor = new Instructor();
        instructor.CourseAssignments = new List<CourseAssignment>();
        PopulateAssignedCourseData(instructor);
        return View();
    }

    // POST: Instructors/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,HireDate,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
    {
        if (selectedCourses != null)
        {
            instructor.CourseAssignments = new List<CourseAssignment>();
            foreach (var course in selectedCourses)
            {
                var courseToAdd = new CourseAssignment
                {
                    InstructorId = instructor.Id,
                    CourseId = int.Parse(course)
                };
                instructor.CourseAssignments.Add(courseToAdd);
            }
        }

        if (ModelState.IsValid)
        {
            _context.Add(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateAssignedCourseData(instructor);
        return View(instructor);
    }

    // GET: Instructors/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Instructors == null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .Include(i => i.OfficeAssignment)
            .Include(i => i.CourseAssignments)
                .ThenInclude(c => c.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
        if (instructor == null)
        {
            return NotFound();
        }

        PopulateAssignedCourseData(instructor);
        return View(instructor);
    }

    // POST: Instructors/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,HireDate")] Instructor instructor)
    //{
    //    if (id != instructor.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(instructor);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!InstructorExists(instructor.Id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction(nameof(Index));
    //    }
    //    return View(instructor);
    //}

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id, string[] selectedCourses)
    {
        if (id == null)
        {
            return NotFound();
        }

        var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(i => i.Id == id);

        if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "",
                i => i.FirstName, i => i.LastName, i => i.HireDate, i => instructorToUpdate.OfficeAssignment))
        {
            if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
            {
                instructorToUpdate.OfficeAssignment = null;
            }

            UpdateInstructorCourses(selectedCourses, instructorToUpdate);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save chagnes.");
            }
        }
        UpdateInstructorCourses(selectedCourses, instructorToUpdate);
        PopulateAssignedCourseData(instructorToUpdate);
        return View(instructorToUpdate);
    }

    // GET: Instructors/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Instructors == null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(m => m.Id == id);
        if (instructor == null)
        {
            return NotFound();
        }

        return View(instructor);
    }

    // POST: Instructors/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Instructors == null)
        {
            return Problem("Entity set 'WestPacificUniversityContext.Instructors'  is null.");
        }

        var instructor = await _context.Instructors
                .Include(i => i.CourseAssignments)
                .SingleAsync(i => i.Id == id);

        var departments = await _context.Departments
                .Where(d => d.InstructorId == id)
                .ToListAsync();
        departments.ForEach(d => d.InstructorId = null);

        if (instructor != null)
        {
            _context.Instructors.Remove(instructor);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool InstructorExists(int id)
    {
      return _context.Instructors.Any(e => e.Id == id);
    }

    private void PopulateAssignedCourseData(Instructor instructor)
    {
        var allCourses = _context.Courses;
        var instructorCourses = new HashSet<int>(
                instructor.CourseAssignments.Select(c => c.CourseId));
        var viewModel = new List<AssignedCourseData>();
        foreach (var course in allCourses)
        {
            viewModel.Add(new AssignedCourseData
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Assigned = instructorCourses.Contains(course.CourseId)
            });
        }
        ViewData["Courses"] = viewModel;
    }

    private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
    {
        if (selectedCourses == null)
        {
            instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
            return;
        }

        var selectedCoursesHS = new HashSet<string>(selectedCourses);
        var instructorCourses = new HashSet<int>
            (instructorToUpdate.CourseAssignments.Select(c => c.Course.CourseId));
        foreach (var course in _context.Courses)
        {
            if (selectedCoursesHS.Contains(course.CourseId.ToString()))
            {
                if (!instructorCourses.Contains(course.CourseId))
                {
                    instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorId = instructorToUpdate.Id, CourseId = course.CourseId });
                }
            }
            else
            {
                if (instructorCourses.Contains(course.CourseId))
                {
                    CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.FirstOrDefault(i => i.CourseId == course.CourseId);
                    _context.Remove(courseToRemove);
                }
            }
        }
    }
}
