using LegalOffice.Infrastructure;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Logging;
using LegalOffice.Web.Filters;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

builder.Logging.AddProvider(new DailyFileLoggerProvider(Path.Combine(builder.Environment.ContentRootPath, "Logs")));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<ICaseTypeOptionsService, CaseTypeOptionsService>();
builder.Services.AddScoped<IWorkflowStatusService, WorkflowStatusService>();
builder.Services.AddScoped<PermissionAuthorizationFilter>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<PermissionAuthorizationFilter>();
}).AddRazorRuntimeCompilation();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var culture = new CultureInfo("ar-EG");
    options.DefaultRequestCulture = new RequestCulture(culture);
    options.SupportedCultures = new[] { culture };
    options.SupportedUICultures = new[] { culture };
});

var app = builder.Build();
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
var applyDatabaseMigrations = builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", app.Environment.IsDevelopment());

app.UseRequestLocalization();

if (applyDatabaseMigrations)
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var services = scope.ServiceProvider;
            var db = services.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
            await DbSeeder.SeedAsync(services);
        }
        catch (Exception ex)
        {
            startupLogger.LogError(ex, "An error occurred during application startup database initialization.");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
        throw;
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.Run();
