using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Exceptions;
using StudentManagement.Application.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Interfaces;
using Xunit;

namespace StudentManagement.Tests.Services;

public class StudentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStudentRepository> _studentRepoMock;
    private readonly Mock<IValidator<CreateStudentDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateStudentDto>> _updateValidatorMock;
    private readonly Mock<ILogger<StudentService>> _loggerMock;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _studentRepoMock = new Mock<IStudentRepository>();
        _createValidatorMock = new Mock<IValidator<CreateStudentDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateStudentDto>>();
        _loggerMock = new Mock<ILogger<StudentService>>();

        _unitOfWorkMock.Setup(u => u.Students).Returns(_studentRepoMock.Object);

        _studentService = new StudentService(
            _unitOfWorkMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllStudentsAsync_ShouldReturnListOfStudents()
    {
        // Arrange
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "John Doe", Email = "john@example.com", Age = 20, Course = "CS", CreatedDate = DateTime.UtcNow },
            new Student { Id = 2, Name = "Jane Doe", Email = "jane@example.com", Age = 22, Course = "IT", CreatedDate = DateTime.UtcNow }
        };

        _studentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetStudentByIdAsync_WhenStudentExists_ShouldReturnStudentDto()
    {
        // Arrange
        var student = new Student { Id = 1, Name = "John Doe", Email = "john@example.com", Age = 20, Course = "CS" };
        _studentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        // Act
        var result = await _studentService.GetStudentByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task GetStudentByIdAsync_WhenStudentDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _studentRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        // Act
        Func<Task> act = async () => await _studentService.GetStudentByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateStudentAsync_WhenValidDtoAndUniqueEmail_ShouldCreateAndReturnStudent()
    {
        // Arrange
        var createDto = new CreateStudentDto { Name = "Alice", Email = "alice@example.com", Age = 21, Course = "Math" };

        _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _studentRepoMock.Setup(r => r.IsEmailUniqueAsync(createDto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _studentService.CreateStudentAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Alice");
        result.Email.Should().Be("alice@example.com");
        _studentRepoMock.Verify(r => r.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateStudentAsync_WhenDuplicateEmail_ShouldThrowDuplicateEmailException()
    {
        // Arrange
        var createDto = new CreateStudentDto { Name = "Alice", Email = "alice@example.com", Age = 21, Course = "Math" };

        _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _studentRepoMock.Setup(r => r.IsEmailUniqueAsync(createDto.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _studentService.CreateStudentAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<DuplicateEmailException>();
    }
}
