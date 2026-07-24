using StudentManagement.Domain.Interfaces;
using StudentManagement.Persistence.Context;

namespace StudentManagement.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IStudentRepository? _students;
    private IUserRepository? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IStudentRepository Students => _students ??= new StudentRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
