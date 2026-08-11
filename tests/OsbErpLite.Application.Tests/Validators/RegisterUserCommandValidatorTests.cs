using FluentValidation.TestHelper;
using OsbErpLite.Application.Identity.Commands;
using OsbErpLite.Application.Identity.Validators;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.Validators;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new RegisterUserCommand("testuser", "P@ssw0rd!", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyUsername_ShouldHaveError()
    {
        var command = new RegisterUserCommand("", "P@ssw0rd!", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void ShortUsername_ShouldHaveError()
    {
        var command = new RegisterUserCommand("ab", "P@ssw0rd!", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void EmptyPassword_ShouldHaveError()
    {
        var command = new RegisterUserCommand("testuser", "", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void ShortPassword_ShouldHaveError()
    {
        var command = new RegisterUserCommand("testuser", "P@ss1", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordWithoutUppercase_ShouldHaveError()
    {
        var command = new RegisterUserCommand("testuser", "p@ssw0rd!", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordWithoutLowercase_ShouldHaveError()
    {
        var command = new RegisterUserCommand("testuser", "P@SSW0RD!", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordWithoutDigit_ShouldHaveError()
    {
        var command = new RegisterUserCommand("testuser", "P@ssword!", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordWithoutSpecialChar_ShouldHaveError()
    {
        var command = new RegisterUserCommand("testuser", "Passw0rds", UserRole.Employee, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void InvalidRole_ShouldHaveError()
    {
        var command = new RegisterUserCommand("testuser", "P@ssw0rd!", (UserRole)99, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Role);
    }
}
