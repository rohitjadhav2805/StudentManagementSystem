namespace StudentManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
