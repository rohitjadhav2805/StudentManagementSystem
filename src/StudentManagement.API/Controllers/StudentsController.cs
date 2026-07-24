using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Common;
using StudentManagement.Domain.Constants;

namespace StudentManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Retrieve all students.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of students.</returns>
    [HttpGet]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var students = await _studentService.GetAllStudentsAsync(cancellationToken);
        var response = ApiResponse<IEnumerable<StudentDto>>.SuccessResponse(students, "Students retrieved successfully.", StatusCodes.Status200OK);
        return Ok(response);
    }

    /// <summary>
    /// Retrieve student by unique ID.
    /// </summary>
    /// <param name="id">Student ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Student details.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetStudentByIdAsync(id, cancellationToken);
        var response = ApiResponse<StudentDto>.SuccessResponse(student, "Student retrieved successfully.", StatusCodes.Status200OK);
        return Ok(response);
    }

    /// <summary>
    /// Search students by name, email, or course.
    /// </summary>
    /// <param name="searchTerm">Search keyword for name or email.</param>
    /// <param name="course">Course filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Filtered list of students.</returns>
    [HttpGet("search")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? searchTerm, [FromQuery] string? course, CancellationToken cancellationToken)
    {
        var students = await _studentService.SearchStudentsAsync(searchTerm, course, cancellationToken);
        var response = ApiResponse<IEnumerable<StudentDto>>.SuccessResponse(students, "Search completed successfully.", StatusCodes.Status200OK);
        return Ok(response);
    }

    /// <summary>
    /// Create a new student (Admin only).
    /// </summary>
    /// <param name="createStudentDto">Student payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Newly created student details.</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto createStudentDto, CancellationToken cancellationToken)
    {
        var createdStudent = await _studentService.CreateStudentAsync(createStudentDto, cancellationToken);
        var response = ApiResponse<StudentDto>.SuccessResponse(createdStudent, "Student created successfully.", StatusCodes.Status201Created);
        return CreatedAtAction(nameof(GetById), new { id = createdStudent.Id }, response);
    }

    /// <summary>
    /// Update existing student details (Admin only).
    /// </summary>
    /// <param name="id">Student ID.</param>
    /// <param name="updateStudentDto">Updated student payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated student details.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto updateStudentDto, CancellationToken cancellationToken)
    {
        var updatedStudent = await _studentService.UpdateStudentAsync(id, updateStudentDto, cancellationToken);
        var response = ApiResponse<StudentDto>.SuccessResponse(updatedStudent, "Student updated successfully.", StatusCodes.Status200OK);
        return Ok(response);
    }

    /// <summary>
    /// Delete student by ID (Admin only).
    /// </summary>
    /// <param name="id">Student ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _studentService.DeleteStudentAsync(id, cancellationToken);
        var response = ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully.", StatusCodes.Status200OK);
        return Ok(response);
    }
}
