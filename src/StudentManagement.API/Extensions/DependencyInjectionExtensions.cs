using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Application.Interfaces;
using StudentManagement.Application.Services;
using StudentManagement.Application.Validators;
using StudentManagement.Domain.Interfaces;
using StudentManagement.Infrastructure.Authentication;
using StudentManagement.Infrastructure.Security;
using StudentManagement.Infrastructure.Settings;
using StudentManagement.Persistence.Context;
using StudentManagement.Persistence.Repositories;

namespace StudentManagement.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Settings
        var jwtSettingsSection = configuration.GetSection(JwtSettings.SectionName);
        services.Configure<JwtSettings>(jwtSettingsSection);
        var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? new JwtSettings
        {
            Secret = "ZestIndiaSecretKeyForJWTAuthenticationProductionReady2026!",
            Issuer = "StudentManagementAPI",
            Audience = "StudentManagementClient",
            ExpiryMinutes = 60
        };

        // Authentication & JWT Bearer Config
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireUserOrAdminRole", policy => policy.RequireRole("Admin", "User"));
        });

        // DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories & Unit of Work
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Infrastructure Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Application Services
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAuthService, AuthService>();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateStudentDtoValidator>();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }
}
