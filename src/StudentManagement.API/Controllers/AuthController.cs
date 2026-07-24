using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Common;

namespace StudentManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticate user and receive JWT bearer token.
    /// </summary>
    /// <param name="loginRequest">User email and password credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JWT Bearer token and user details.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(loginRequest, cancellationToken);
        var response = ApiResponse<LoginResponseDto>.SuccessResponse(result, "User authenticated successfully.", StatusCodes.Status200OK);
        return Ok(response);
    }

    /// <summary>
    /// Register a new user with role (Admin or User).
    /// </summary>
    /// <param name="registerRequest">User registration details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JWT Bearer token and new user details.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(registerRequest, cancellationToken);
        var response = ApiResponse<LoginResponseDto>.SuccessResponse(result, "User registered successfully.", StatusCodes.Status201Created);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
