using AutoMapper;
using FluentAssertions;
using Moq;
using OsbErpLite.Application.BusinessPartners.Commands;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.BusinessPartners;

public class CreateBusinessPartnerCommandHandlerTests
{
    private readonly Mock<IBusinessPartnerRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateBusinessPartnerCommandHandler _handler;

    public CreateBusinessPartnerCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBusinessPartnerRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateBusinessPartnerCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateBusinessPartner()
    {
        var command = new CreateBusinessPartnerCommand(
            "Test Supplier", "1234567890", BusinessPartnerType.Supplier,
            "test@test.com", "555-0100", "Test Address", "Istanbul");

        var entity = new BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            TaxNumber = command.TaxNumber,
            Type = command.Type,
            IsActive = true
        };

        var expectedDto = new BusinessPartnerDto
        {
            Id = entity.Id,
            Name = entity.Name,
            TaxNumber = entity.TaxNumber,
            Type = entity.Type,
            IsActive = true
        };

        _mapperMock.Setup(m => m.Map<BusinessPartner>(command)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<BusinessPartnerDto>(entity)).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test Supplier");
        result.Type.Should().Be(BusinessPartnerType.Supplier);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<BusinessPartner>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateTaxNumber_ShouldStillCallRepository()
    {
        var command = new CreateBusinessPartnerCommand(
            "Duplicate Partner", "9876543210", BusinessPartnerType.Customer,
            null, null, null, null);

        var entity = new BusinessPartner { Id = Guid.NewGuid(), Name = command.Name, TaxNumber = command.TaxNumber };
        var dto = new BusinessPartnerDto { Id = entity.Id, Name = entity.Name, TaxNumber = entity.TaxNumber };

        _mapperMock.Setup(m => m.Map<BusinessPartner>(command)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<BusinessPartnerDto>(entity)).Returns(dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.TaxNumber.Should().Be("9876543210");
    }
}
