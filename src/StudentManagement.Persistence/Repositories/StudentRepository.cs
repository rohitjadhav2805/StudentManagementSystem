using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Interfaces;
using StudentManagement.Persistence.Context;

namespace StudentManagement.Persistence.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLower();
        if (excludeId.HasValue)
        {
            return !await DbSet.AnyAsync(s => s.Email.ToLower() == normalizedEmail && s.Id != excludeId.Value, cancellationToken);
        }
        return !await DbSet.AnyAsync(s => s.Email.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task<IEnumerable<Student>> SearchStudentsAsync(string? searchTerm, string? course, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(term) || s.Email.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(course))
        {
            var courseTerm = course.Trim().ToLower();
            query = query.Where(s => s.Course.ToLower().Contains(courseTerm));
        }

        return await query.OrderByDescending(s => s.CreatedDate).ToListAsync(cancellationToken);
    }
}
