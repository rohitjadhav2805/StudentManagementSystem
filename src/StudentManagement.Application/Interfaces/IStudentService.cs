using StudentManagement.Application.DTOs.Student;

namespace StudentManagement.Application.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default);
    Task<StudentDto> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentDto>> SearchStudentsAsync(string? searchTerm, string? course, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto, CancellationToken cancellationToken = default);
    Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto updateStudentDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken = default);
}
