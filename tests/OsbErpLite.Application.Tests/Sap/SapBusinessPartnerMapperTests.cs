using FluentAssertions;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Enums;
using OsbErpLite.Infrastructure.ExternalServices.Sap;

namespace OsbErpLite.Application.Tests.Sap;

public class SapBusinessPartnerMapperTests
{
    [Fact]
    public void MapToEntity_WithValidRecord_ShouldMapCorrectly()
    {
        var record = new SapBusinessPartnerRecord
        {
            Name = "SAP Company",
            TaxNumber = "9876543210",
            Category = "1",
            SapId = "BP-001"
        };

        var entity = SapBusinessPartnerMapper.MapToEntity(record);

        entity.Name.Should().Be("SAP Company");
        entity.TaxNumber.Should().Be("9876543210");
        entity.Type.Should().Be(BusinessPartnerType.Supplier);
        entity.IsActive.Should().BeTrue();
        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void MapToEntity_NullName_ShouldFallbackToSapId()
    {
        var record = new SapBusinessPartnerRecord
        {
            Name = null,
            TaxNumber = "1234567890",
            Category = "2",
            SapId = "BP-002"
        };

        var entity = SapBusinessPartnerMapper.MapToEntity(record);

        entity.Name.Should().Be("BP-002");
    }

    [Fact]
    public void MapToEntity_NullNameAndSapId_ShouldFallbackToUnknown()
    {
        var record = new SapBusinessPartnerRecord
        {
            Name = null,
            TaxNumber = "1111111111",
            Category = "2",
            SapId = null
        };

        var entity = SapBusinessPartnerMapper.MapToEntity(record);

        entity.Name.Should().Be("Unknown");
    }

    [Theory]
    [InlineData("1", BusinessPartnerType.Supplier)]
    [InlineData("2", BusinessPartnerType.Customer)]
    [InlineData("3", BusinessPartnerType.Customer)]
    [InlineData(null, BusinessPartnerType.Customer)]
    [InlineData("", BusinessPartnerType.Customer)]
    public void MapToEntity_Category_ShouldMapCorrectly(string? category, BusinessPartnerType expected)
    {
        var record = new SapBusinessPartnerRecord
        {
            Name = "Test",
            TaxNumber = "1234567890",
            Category = category,
            SapId = "BP-X"
        };

        var entity = SapBusinessPartnerMapper.MapToEntity(record);

        entity.Type.Should().Be(expected);
    }

    [Fact]
    public void UpdateEntity_ShouldUpdateFields()
    {
        var entity = new Domain.Entities.BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            TaxNumber = "1234567890",
            Type = BusinessPartnerType.Customer
        };
        var record = new SapBusinessPartnerRecord
        {
            Name = "New Name",
            TaxNumber = "1234567890",
            Category = "1"
        };

        SapBusinessPartnerMapper.UpdateEntity(entity, record);

        entity.Name.Should().Be("New Name");
        entity.Type.Should().Be(BusinessPartnerType.Supplier);
        entity.UpdatedDate.Should().NotBeNull();
    }

    [Fact]
    public void UpdateEntity_NullName_ShouldKeepExistingName()
    {
        var entity = new Domain.Entities.BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = "Keep This Name",
            Type = BusinessPartnerType.Customer
        };
        var record = new SapBusinessPartnerRecord
        {
            Name = null,
            SapId = null,
            Category = "2"
        };

        SapBusinessPartnerMapper.UpdateEntity(entity, record);

        entity.Name.Should().Be("Keep This Name");
    }
}
