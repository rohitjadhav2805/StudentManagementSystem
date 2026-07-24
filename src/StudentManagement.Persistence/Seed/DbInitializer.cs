using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Constants;
using StudentManagement.Domain.Entities;
using StudentManagement.Persistence.Context;

namespace StudentManagement.Persistence.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        try
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                var _ = await context.Users.AnyAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation("Database tables do not exist yet. Initializing database schema...");
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("Seeding default Users...");

                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@zestindia.com",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"),
                    Role = UserRoles.Admin,
                    CreatedDate = DateTime.UtcNow
                };

                var standardUser = new User
                {
                    Username = "user",
                    Email = "user@zestindia.com",
                    PasswordHash = passwordHasher.HashPassword("User@123"),
                    Role = UserRoles.User,
                    CreatedDate = DateTime.UtcNow
                };

                await context.Users.AddRangeAsync(adminUser, standardUser);
                await context.SaveChangesAsync();
                logger.LogInformation("Users seeded successfully.");
            }
            else
            {
                // Ensure existing seeded users have valid password hashes
                var existingAdmin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@zestindia.com");
                if (existingAdmin != null && !passwordHasher.VerifyPassword("Admin@123", existingAdmin.PasswordHash))
                {
                    existingAdmin.PasswordHash = passwordHasher.HashPassword("Admin@123");
                    context.Users.Update(existingAdmin);
                }

                var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "user@zestindia.com");
                if (existingUser != null && !passwordHasher.VerifyPassword("User@123", existingUser.PasswordHash))
                {
                    existingUser.PasswordHash = passwordHasher.HashPassword("User@123");
                    context.Users.Update(existingUser);
                }

                await context.SaveChangesAsync();
            }

            if (!await context.Students.AnyAsync())
            {
                logger.LogInformation("Seeding default Students...");

                var students = new List<Student>
                {
                    new Student
                    {
                        Name = "Rahul Sharma",
                        Email = "rahul.sharma@example.com",
                        Age = 22,
                        Course = "Computer Science",
                        CreatedDate = DateTime.UtcNow.AddDays(-30)
                    },
                    new Student
                    {
                        Name = "Priya Patel",
                        Email = "priya.patel@example.com",
                        Age = 21,
                        Course = "Information Technology",
                        CreatedDate = DateTime.UtcNow.AddDays(-25)
                    },
                    new Student
                    {
                        Name = "Amit Kumar",
                        Email = "amit.kumar@example.com",
                        Age = 23,
                        Course = "Software Engineering",
                        CreatedDate = DateTime.UtcNow.AddDays(-20)
                    },
                    new Student
                    {
                        Name = "Sneha Verma",
                        Email = "sneha.verma@example.com",
                        Age = 20,
                        Course = "Data Science",
                        CreatedDate = DateTime.UtcNow.AddDays(-15)
                    },
                    new Student
                    {
                        Name = "Vikram Singh",
                        Email = "vikram.singh@example.com",
                        Age = 24,
                        Course = "Artificial Intelligence",
                        CreatedDate = DateTime.UtcNow.AddDays(-10)
                    }
                };

                await context.Students.AddRangeAsync(students);
                await context.SaveChangesAsync();
                logger.LogInformation("Students seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
