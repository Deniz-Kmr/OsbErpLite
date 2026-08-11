using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Identity.Commands;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.Identity;

public class ChangeOwnPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ILogger<ChangeOwnPasswordCommandHandler>> _loggerMock;
    private readonly ChangeOwnPasswordCommandHandler _handler;

    public ChangeOwnPasswordCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<ChangeOwnPasswordCommandHandler>>();
        _handler = new ChangeOwnPasswordCommandHandler(_userRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CorrectCurrentPassword_ShouldChangePassword()
    {
        var userId = Guid.NewGuid();
        var currentPassword = "OldP@ssw0rd";
        var newPassword = "NewP@ssw0rd!";
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(currentPassword),
            Role = UserRole.Employee,
            IsActive = true
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new ChangeOwnPasswordCommand(userId, currentPassword, newPassword);
        await _handler.Handle(command, CancellationToken.None);

        BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash).Should().BeTrue();
        _userRepoMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongCurrentPassword_ShouldThrowUnauthorized()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ActualPassword"),
            Role = UserRole.Employee,
            IsActive = true
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var command = new ChangeOwnPasswordCommand(userId, "WrongPassword", "NewP@ss1");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*hatalı*");
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldThrowKeyNotFound()
    {
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new ChangeOwnPasswordCommand(userId, "any", "any");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
