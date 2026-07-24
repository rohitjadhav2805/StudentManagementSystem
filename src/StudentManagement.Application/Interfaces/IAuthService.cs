using StudentManagement.Application.DTOs.Auth;

namespace StudentManagement.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken = default);
}
