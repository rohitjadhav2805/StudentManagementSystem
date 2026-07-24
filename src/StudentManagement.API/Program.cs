using Serilog;
using StudentManagement.API.Extensions;
using StudentManagement.API.Middleware;
using StudentManagement.Application.Interfaces;
using StudentManagement.Persistence.Context;
using StudentManagement.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// Serilog Setup
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Configure default URLs if ASPNETCORE_URLS is not set
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5242;http://localhost:5000");
}

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Student Management API application...");

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddSwaggerDocumentation();

    var app = builder.Build();

    // Middleware Pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();

    if (app.Environment.IsDevelopment() || true) // Enable Swagger in all environments for testing ease
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API v1");
            c.RoutePrefix = "swagger";
            c.DisplayRequestDuration();
        });
    }

    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Database Migration & Seeding
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();
            await DbInitializer.SeedAsync(dbContext, passwordHasher, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database migration/seeding.");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
