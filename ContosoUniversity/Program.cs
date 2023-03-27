using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ContosoUniversity.Data;

var builder = WebApplication.CreateBuilder(args);

// DbContext is registered to DI container as a scoped service (aligned with HTTP request lifttime)
builder.Services.AddDbContext<ContosoUniversityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContosoUniversityContext") 
        ?? throw new InvalidOperationException("Connection string 'ContosoUniversityContext' not found.")
));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddControllersWithViews();
    //.AddRazorRuntimeCompilation(); // runtime optimization breaks "scoped CSS isolation" introduced in .NET6.0

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var scopedServices = scope.ServiceProvider;

    try
    {
        var context = scopedServices.GetRequiredService<ContosoUniversityContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = scopedServices.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
