using FluentValidation;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Exceptions;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Interfaces;

namespace StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateStudentDto> _createValidator;
    private readonly IValidator<UpdateStudentDto> _updateValidator;
    private readonly ILogger<StudentService> _logger;

    public StudentService(
        IUnitOfWork unitOfWork,
        IValidator<CreateStudentDto> createValidator,
        IValidator<UpdateStudentDto> updateValidator,
        ILogger<StudentService> logger)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all students");
        var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);
        return students.Select(MapToDto);
    }

    public async Task<StudentDto> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching student with ID: {StudentId}", id);
        var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
        if (student == null)
        {
            _logger.LogWarning("Student with ID: {StudentId} not found", id);
            throw new NotFoundException(nameof(Student), id);
        }

        return MapToDto(student);
    }

    public async Task<IEnumerable<StudentDto>> SearchStudentsAsync(string? searchTerm, string? course, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching students with SearchTerm: '{SearchTerm}', Course: '{Course}'", searchTerm, course);
        var students = await _unitOfWork.Students.SearchStudentsAsync(searchTerm, course, cancellationToken);
        return students.Select(MapToDto);
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new student with Email: {Email}", createStudentDto.Email);

        var validationResult = await _createValidator.ValidateAsync(createStudentDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Validation failed for CreateStudentDto: {Errors}", string.Join(", ", errors));
            throw new BadRequestException("Validation failed", errors);
        }

        bool isUnique = await _unitOfWork.Students.IsEmailUniqueAsync(createStudentDto.Email, null, cancellationToken);
        if (!isUnique)
        {
            _logger.LogWarning("Student creation failed - duplicate email: {Email}", createStudentDto.Email);
            throw new DuplicateEmailException(createStudentDto.Email);
        }

        var student = new Student
        {
            Name = createStudentDto.Name,
            Email = createStudentDto.Email,
            Age = createStudentDto.Age,
            Course = createStudentDto.Course,
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Students.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created student with ID: {StudentId}", student.Id);
        return MapToDto(student);
    }

    public async Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto updateStudentDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating student with ID: {StudentId}", id);

        var validationResult = await _updateValidator.ValidateAsync(updateStudentDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Validation failed for UpdateStudentDto: {Errors}", string.Join(", ", errors));
            throw new BadRequestException("Validation failed", errors);
        }

        var existingStudent = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
        if (existingStudent == null)
        {
            _logger.LogWarning("Student update failed - Student ID: {StudentId} not found", id);
            throw new NotFoundException(nameof(Student), id);
        }

        bool isUnique = await _unitOfWork.Students.IsEmailUniqueAsync(updateStudentDto.Email, id, cancellationToken);
        if (!isUnique)
        {
            _logger.LogWarning("Student update failed - duplicate email: {Email}", updateStudentDto.Email);
            throw new DuplicateEmailException(updateStudentDto.Email);
        }

        existingStudent.Name = updateStudentDto.Name;
        existingStudent.Email = updateStudentDto.Email;
        existingStudent.Age = updateStudentDto.Age;
        existingStudent.Course = updateStudentDto.Course;

        _unitOfWork.Students.Update(existingStudent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated student with ID: {StudentId}", id);
        return MapToDto(existingStudent);
    }

    public async Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting student with ID: {StudentId}", id);

        var existingStudent = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
        if (existingStudent == null)
        {
            _logger.LogWarning("Student deletion failed - Student ID: {StudentId} not found", id);
            throw new NotFoundException(nameof(Student), id);
        }

        _unitOfWork.Students.Remove(existingStudent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted student with ID: {StudentId}", id);
        return true;
    }

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age,
            Course = student.Course,
            CreatedDate = student.CreatedDate
        };
    }
}
