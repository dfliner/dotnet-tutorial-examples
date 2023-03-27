﻿using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ContosoUniversity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ContosoUniversityContext _dbContext;

    public HomeController(ILogger<HomeController> logger, ContosoUniversityContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> About()
    {
        var data = from s in _dbContext.Students
                   group s by s.EnrollmentDate into dataGroup
                   select new EnrollmentDateGroup()
                   {
                       EnrollmentDate = dataGroup.Key,
                       StudentCount = dataGroup.Count()
                   };
        return View(await data.AsNoTracking().ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
