using WestPacificUniversity.Data;
using WestPacificUniversity.Models;
using WestPacificUniversity.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WestPacificUniversity.Controllers;

public class CoursesController : Controller
{
    private readonly WestPacificUniversityContext _context;
    private readonly IConfiguration _configuration;

    public CoursesController(WestPacificUniversityContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // GET: Courses
    public async Task<IActionResult> Index(
        string? sortOrder,
        int? departmentId,
        string? searchString,
        string? currentFilter,
        int? pageNumber)
    {
        sortOrder = sortOrder?.ToLowerInvariant();
        searchString = searchString ?? currentFilter;

        var departments = _context.Departments.Distinct().Select(d => new { d.Id, d.Name }).AsNoTracking();
        var VM = new ListOfCoursesViewModel
        {
            CurrentSort = sortOrder,
            CurrentFilter = searchString,
            DepartmentId = departmentId,
            Departments = new SelectList(await departments.ToListAsync(), "Id", "Name", departmentId),
        };

        searchString = searchString ?? currentFilter;

        IQueryable<Course> courses = _context.Courses.Include(c => c.Department);
        if (departmentId.HasValue)
        {
            courses = courses.Where(c => c.DepartmentId == departmentId.Value);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            courses = courses.Where(c => c.Title.Contains(searchString));
        }
        VM.Courses = await PaginatedList<Course>.CreateAsync(
            courses.AsNoTracking(),
            pageNumber ?? 1,
            _configuration.GetValue("PageSize", 4));

        return View(VM);
    }

    // GET: Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Courses == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CourseId == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // GET: Courses/Create
    public IActionResult Create()
    {
        PopulateDepartmentsDropList();
        return View();
    }

    // POST: Courses/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CourseId,Title,Credit,DepartmentId")] Course course)
    {
        if (ModelState.IsValid)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateDepartmentsDropList(course.DepartmentId);
        return View(course);
    }

    // GET: Courses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Courses == null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        PopulateDepartmentsDropList(course.DepartmentId);
        return View(course);
    }

    // POST: Courses/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,Credit,DepartmentId")] Course course)
    //{
    //    if (id != course.CourseId)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(course);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!CourseExists(course.CourseId))
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
    //    ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", course.DepartmentId);
    //    return View(course);
    //}

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var courseToUpdate = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseId == id);

        // Only the specified properties can be updated and only the modified properties are updated
        if (await TryUpdateModelAsync<Course>(courseToUpdate, "", c => c.Title, c => c.DepartmentId, c => c.Credit))
        {
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

        PopulateDepartmentsDropList(courseToUpdate.DepartmentId);
        return View(courseToUpdate);
    }

    // GET: Courses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Courses == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CourseId == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // POST: Courses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Courses == null)
        {
            return Problem("Entity set 'WestPacificUniversityContext.Courses'  is null.");
        }
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int id)
    {
      return _context.Courses.Any(e => e.CourseId == id);
    }

    private void PopulateDepartmentsDropList(object selectedDepartment = null)
    {
        var departmentsQuery = from d in _context.Departments
                               orderby d.Name
                               select d;
        //var query = _context.Departments.OrderBy(d => d.Name);

        ViewBag.DepartmentId = new SelectList(departmentsQuery.AsNoTracking(), nameof(Department.Id), nameof(Department.Name), selectedDepartment);
    }
}
