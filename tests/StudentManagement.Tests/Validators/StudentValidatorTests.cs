using FluentAssertions;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Validators;
using Xunit;

namespace StudentManagement.Tests.Validators;

public class StudentValidatorTests
{
    private readonly CreateStudentDtoValidator _createValidator = new();
    private readonly UpdateStudentDtoValidator _updateValidator = new();

    [Fact]
    public void CreateStudentDtoValidator_WhenValidPayload_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateStudentDto
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 22,
            Course = "Software Engineering"
        };

        // Act
        var result = _createValidator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void CreateStudentDtoValidator_WhenInvalidEmailAndAgeZero_ShouldFailValidation()
    {
        // Arrange
        var dto = new CreateStudentDto
        {
            Name = "",
            Email = "invalid-email-format",
            Age = 0,
            Course = ""
        };

        // Act
        var result = _createValidator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
    }
}
