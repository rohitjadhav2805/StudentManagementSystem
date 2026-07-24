using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentManagement.API.Controllers;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Common;
using Xunit;

namespace StudentManagement.Tests.Controllers;

public class StudentsControllerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        _controller = new StudentsController(_studentServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkObjectResultWithSuccessResponse()
    {
        // Arrange
        var students = new List<StudentDto>
        {
            new StudentDto { Id = 1, Name = "Alice", Email = "alice@example.com", Age = 20, Course = "CS" }
        };

        _studentServiceMock.Setup(s => s.GetAllStudentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<StudentDto>>>().Subject;

        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(1);
        apiResponse.StatusCode.Should().Be(200);
    }
}
