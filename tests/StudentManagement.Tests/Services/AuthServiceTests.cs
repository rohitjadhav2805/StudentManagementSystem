using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Exceptions;
using StudentManagement.Application.Interfaces;
using StudentManagement.Application.Services;
using StudentManagement.Domain.Constants;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Interfaces;
using Xunit;

namespace StudentManagement.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IValidator<LoginRequestDto>> _loginValidatorMock;
    private readonly Mock<IValidator<RegisterRequestDto>> _registerValidatorMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _loginValidatorMock = new Mock<IValidator<LoginRequestDto>>();
        _registerValidatorMock = new Mock<IValidator<RegisterRequestDto>>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        _authService = new AuthService(
            _unitOfWorkMock.Object,
            _jwtGeneratorMock.Object,
            _passwordHasherMock.Object,
            _loginValidatorMock.Object,
            _registerValidatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginRequestDto { Email = "admin@zestindia.com", Password = "Admin@123" };
        var user = new User { Id = 1, Email = "admin@zestindia.com", Username = "admin", PasswordHash = "hashed_pass", Role = UserRoles.Admin };

        _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userRepoMock.Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(p => p.VerifyPassword(loginDto.Password, user.PasswordHash))
            .Returns(true);

        _jwtGeneratorMock.Setup(j => j.GenerateToken(user))
            .Returns(("jwt_sample_token", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt_sample_token");
        result.Email.Should().Be("admin@zestindia.com");
        result.Role.Should().Be(UserRoles.Admin);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginDto = new LoginRequestDto { Email = "nonexistent@example.com", Password = "Password123" };

        _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userRepoMock.Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
