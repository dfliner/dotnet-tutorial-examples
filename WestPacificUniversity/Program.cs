using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WestPacificUniversity.Data;
using WestPacificUniversity.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(container =>
    {
        container.RegisterModule(new ApplicationModule());
    });

// DbContext is registered to DI container as a scoped service (aligned with HTTP request lifetime)
builder.Services.AddDbContext<WestPacificUniversityContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("WestPacificUniversityContext")
        ?? throw new InvalidOperationException("Connection string 'WestPacificUniversityContext' not found.")
    ));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

// Add services to the container.
builder.Services.AddControllersWithViews()
    // Runtime optimization breaks "scoped CSS isolation" introduced in .NET6.0
    // https://github.com/dotnet/aspnetcore/issues/33446
    // To mitigate the issue, we use style sections for page-scoped styles,
    // and also move content of _layout.cshtml.css to the site.css.
    .AddRazorRuntimeCompilation(); 

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var scopedServices = scope.ServiceProvider;

    try
    {
        var context = scopedServices.GetRequiredService<WestPacificUniversityContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = scopedServices.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios,
    // see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
