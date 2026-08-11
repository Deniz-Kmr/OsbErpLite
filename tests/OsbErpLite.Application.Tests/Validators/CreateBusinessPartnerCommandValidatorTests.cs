using FluentAssertions;
using FluentValidation.TestHelper;
using OsbErpLite.Application.BusinessPartners.Commands;
using OsbErpLite.Application.BusinessPartners.Validators;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.Validators;

public class CreateBusinessPartnerCommandValidatorTests
{
    private readonly CreateBusinessPartnerCommandValidator _validator;

    public CreateBusinessPartnerCommandValidatorTests()
    {
        _validator = new CreateBusinessPartnerCommandValidator();
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new CreateBusinessPartnerCommand(
            "Test Partner", "1234567890", BusinessPartnerType.Customer,
            "test@test.com", "555-0100", "Address", "Istanbul");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_ShouldHaveError()
    {
        var command = new CreateBusinessPartnerCommand(
            "", "1234567890", BusinessPartnerType.Customer,
            null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameExceeds200Chars_ShouldHaveError()
    {
        var longName = new string('A', 201);
        var command = new CreateBusinessPartnerCommand(
            longName, null, BusinessPartnerType.Customer,
            null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void InvalidTaxNumber_ShouldHaveError()
    {
        var command = new CreateBusinessPartnerCommand(
            "Test", "ABC", BusinessPartnerType.Customer,
            null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TaxNumber);
    }

    [Fact]
    public void NullTaxNumber_ShouldNotHaveError()
    {
        var command = new CreateBusinessPartnerCommand(
            "Test", null, BusinessPartnerType.Customer,
            null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TaxNumber);
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("12345678901")]
    public void ValidTaxNumber_ShouldNotHaveError(string taxNumber)
    {
        var command = new CreateBusinessPartnerCommand(
            "Test", taxNumber, BusinessPartnerType.Customer,
            null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TaxNumber);
    }

    [Fact]
    public void InvalidEmail_ShouldHaveError()
    {
        var command = new CreateBusinessPartnerCommand(
            "Test", null, BusinessPartnerType.Customer,
            "not-an-email", null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void InvalidEnumType_ShouldHaveError()
    {
        var command = new CreateBusinessPartnerCommand(
            "Test", null, (BusinessPartnerType)99,
            null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Type);
    }
}
