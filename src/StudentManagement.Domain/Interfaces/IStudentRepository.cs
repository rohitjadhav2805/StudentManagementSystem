using StudentManagement.Domain.Entities;

namespace StudentManagement.Domain.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> SearchStudentsAsync(string? searchTerm, string? course, CancellationToken cancellationToken = default);
}
