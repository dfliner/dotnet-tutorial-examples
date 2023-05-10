using WestPacificUniversity.Data;
using WestPacificUniversity.Models;
using WestPacificUniversity.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestPacificUniversity.Data.Entities;

namespace WestPacificUniversity.Controllers;

public class StudentsController : Controller
{
    private readonly WestPacificUniversityContext _context;
    private readonly IConfiguration _configuration;

    public StudentsController(WestPacificUniversityContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }


    // GET: Students
    public async Task<IActionResult> Index(
        string? sortOrder, // sorted head and sort order
        string? searchString, // new search string
        string? currentFilter, // current filtering string (during sorting and paging)
        int? pageNumber)
    {
        sortOrder = sortOrder?.ToLowerInvariant();
        ViewData["CurrentSort"] = sortOrder;
        // The params for next view display.
        // Default sort order is "", meaning "name_asc". 
        ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DateSortParam"] = "date".Equals(sortOrder, StringComparison.Ordinal) ? "date_desc" : "date";

        // If it is not a new search, previous search filter remains.
        searchString = searchString ?? currentFilter;
        ViewData["CurrentFilter"] = searchString;

        IQueryable<Student> students = _context.Students;
        if (!string.IsNullOrEmpty(searchString))
        {
            students = students.Where(s => s.LastName.Contains(searchString)
                                        || s.FirstName.Contains(searchString));
        }

        switch (sortOrder)
        {
            case "date_desc":
                ViewData["NameSortIcon"] = "invisible";
                ViewData["DateSortIcon"] = "fa-sort-down";
                students = students.OrderByDescending(s => s.EnrollmentDate);
                break;
            case "date":
                ViewData["NameSortIcon"] = "invisible";
                ViewData["DateSortIcon"] = "fa-sort-up";
                students = students.OrderBy(s => s.EnrollmentDate);
                break;
            case "name_desc":
                ViewData["NameSortIcon"] = "fa-sort-down";
                ViewData["DateSortIcon"] = "invisible";
                students = students.OrderByDescending(s => s.LastName);
                break;
            default:
                ViewData["NameSortIcon"] = "fa-sort-up";
                ViewData["DateSortIcon"] = "invisible";
                students = students.OrderBy(s => s.LastName);
                break;
        }

        var pageSize = _configuration.GetValue("PageSize", 4);

        // Returns a paginated list to the view
        return View(
            await PaginatedList<Student>.CreateAsync(
                students.AsNoTracking(), 
                pageNumber ?? 1, 
                pageSize));
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Students == null)
        {
            return NotFound();
        }

        var student = await _context.Students
                .Include(s => s.Enrollments)!
                    .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: Students/Create
    public IActionResult Create()
    {
        // Show an empty create page
        return View();
    }

    // POST: Students/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentInput student)
    {
        var studentToCreate = new Student()
        {
            LastName = student.LastName,
            FirstName = student.FirstName,
            EnrollmentDate = student.EnrollmentDate,
        };

        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(studentToCreate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes.");
            }
        }
        return View(studentToCreate);
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Students == null)
        {
            return NotFound();
        }

        // FindAsync is more performant than FirstOrDefaultAsync
        // when populating navigation properties is not needed.
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: Students/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudentInput student)
    {
        if (id != student.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var studentToUpdate = new Student()
                {
                    Id = id,
                    LastName = student.LastName,
                    FirstName = student.FirstName,
                    EnrollmentDate = student.EnrollmentDate
                };

                _context.Update(studentToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                if (!StudentExists(student.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "Unable to save changes. Try again, and if the problem persists, see your sysadmin.");
                }
            }
        }

        // Return to the same edit page if update failed.
        return View(student);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
    {
        if (id == null || _context.Students == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
        {
            return NotFound();
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ViewData["ErrorMessage"] = $"Delete {id} failed. Try again.";
        }

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Students == null)
        {
            return Problem("Entity set 'WestPacificUniversityContext.Student'  is null.");
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        try
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
        }
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }
}
