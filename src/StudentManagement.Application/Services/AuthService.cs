using FluentValidation;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Exceptions;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Interfaces;

namespace StudentManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IValidator<LoginRequestDto> loginValidator,
        IValidator<RegisterRequestDto> registerValidator,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authentication attempt for email: {Email}", loginRequest.Email);

        var validationResult = await _loginValidator.ValidateAsync(loginRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Login validation failed for email: {Email}", loginRequest.Email);
            throw new BadRequestException("Validation failed", errors);
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(loginRequest.Email, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Login failed - user not found: {Email}", loginRequest.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        bool isPasswordValid = _passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Login failed - invalid password for email: {Email}", loginRequest.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        _logger.LogInformation("User logged in successfully: {Email}, Role: {Role}", user.Email, user.Role);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", registerRequest.Email);

        var validationResult = await _registerValidator.ValidateAsync(registerRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Registration validation failed for email: {Email}", registerRequest.Email);
            throw new BadRequestException("Validation failed", errors);
        }

        bool isEmailUnique = await _unitOfWork.Users.IsEmailUniqueAsync(registerRequest.Email, cancellationToken);
        if (!isEmailUnique)
        {
            _logger.LogWarning("Registration failed - email already in use: {Email}", registerRequest.Email);
            throw new DuplicateEmailException(registerRequest.Email);
        }

        var user = new User
        {
            Username = registerRequest.Username,
            Email = registerRequest.Email,
            PasswordHash = _passwordHasher.HashPassword(registerRequest.Password),
            Role = registerRequest.Role,
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }
}
