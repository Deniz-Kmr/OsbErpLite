using FluentAssertions;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Identity.Commands;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.Identity;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
        _handler = new LoginCommandHandler(
            _userRepoMock.Object,
            _jwtServiceMock.Object,
            _refreshTokenRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnToken()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            IsActive = true
        };
        var expiry = DateTime.UtcNow.AddHours(1);

        _userRepoMock.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(s => s.GenerateToken(user)).Returns("mock-jwt-token");
        _jwtServiceMock.Setup(s => s.GetAccessTokenExpiry()).Returns(expiry);
        _jwtServiceMock.Setup(s => s.GenerateRefreshToken()).Returns("mock-refresh-token");
        _jwtServiceMock.Setup(s => s.GetRefreshTokenExpiryDays()).Returns(7);
        _refreshTokenRepoMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken rt, CancellationToken _) => rt);

        var command = new LoginCommand("admin", "CorrectPassword");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("mock-jwt-token");
        result.RefreshToken.Should().Be("mock-refresh-token");
        result.Username.Should().Be("admin");
        result.Role.Should().Be("Admin");
        result.ExpiresAt.Should().Be(expiry);
    }

    [Fact]
    public async Task Handle_WrongPassword_ShouldThrowKeyNotFound()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            IsActive = true
        };

        _userRepoMock.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new LoginCommand("admin", "WrongPassword");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Invalid username or password*");
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldThrowKeyNotFound()
    {
        _userRepoMock.Setup(r => r.GetByUsernameAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginCommand("unknown", "anyPassword");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeactivatedUser_ShouldThrowUnauthorized()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "deactivated",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
            Role = UserRole.Employee,
            IsActive = false
        };

        _userRepoMock.Setup(r => r.GetByUsernameAsync("deactivated", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new LoginCommand("deactivated", "pass");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*deactivated*");
    }
}
