using FluentValidation.TestHelper;
using OsbErpLite.Application.HumanResources.Employees.Commands;
using OsbErpLite.Application.HumanResources.Employees.Validators;

namespace OsbErpLite.Application.Tests.Validators;

public class CreateEmployeeCommandValidatorTests
{
    private readonly CreateEmployeeCommandValidator _validator;

    public CreateEmployeeCommandValidatorTests()
    {
        _validator = new CreateEmployeeCommandValidator();
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new CreateEmployeeCommand(
            "Ali Veli", "10000000146",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyFullName_ShouldHaveError()
    {
        var command = new CreateEmployeeCommand(
            "", "10000000146",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void EmptyNationalId_ShouldHaveError()
    {
        var command = new CreateEmployeeCommand(
            "Test", "",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NationalId);
    }

    [Fact]
    public void NationalIdWrongLength_ShouldHaveError()
    {
        var command = new CreateEmployeeCommand(
            "Test", "1234567890",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NationalId);
    }

    [Fact]
    public void NationalIdNonNumeric_ShouldHaveError()
    {
        var command = new CreateEmployeeCommand(
            "Test", "ABCDEFGHIJK",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NationalId);
    }

    [Fact]
    public void InvalidTcKimlikNo_ShouldHaveError()
    {
        var command = new CreateEmployeeCommand(
            "Test", "12345678901",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NationalId);
    }

    [Fact]
    public void ValidTcKimlikNo_ShouldNotHaveError()
    {
        var command = new CreateEmployeeCommand(
            "Test", "10000000146",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NationalId);
    }

    [Fact]
    public void ZeroSalary_ShouldHaveError()
    {
        var command = new CreateEmployeeCommand(
            "Test", "10000000146",
            Guid.NewGuid(), Guid.NewGuid(), null,
            DateTime.UtcNow, 0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void EmptyDepartmentId_ShouldHaveError()
    {
        var command = new CreateEmployeeCommand(
            "Test", "10000000146",
            Guid.Empty, Guid.NewGuid(), null,
            DateTime.UtcNow, 15000m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
    }
}
