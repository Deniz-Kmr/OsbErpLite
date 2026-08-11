using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OsbErpLite.Application.BusinessPartners.Commands;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Tests.Sap;

public class SyncBusinessPartnersFromSapCommandHandlerTests
{
    private readonly Mock<ISapBusinessPartnerClient> _sapClientMock;
    private readonly Mock<IBusinessPartnerRepository> _repositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ILogger<SyncBusinessPartnersFromSapCommandHandler>> _loggerMock;
    private readonly SyncBusinessPartnersFromSapCommandHandler _handler;

    public SyncBusinessPartnersFromSapCommandHandlerTests()
    {
        _sapClientMock = new Mock<ISapBusinessPartnerClient>();
        _repositoryMock = new Mock<IBusinessPartnerRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILogger<SyncBusinessPartnersFromSapCommandHandler>>();
        _currentUserMock.Setup(c => c.UserName).Returns("admin");
        _handler = new SyncBusinessPartnersFromSapCommandHandler(
            _sapClientMock.Object, _repositoryMock.Object,
            _currentUserMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NewRecords_ShouldAddToRepository()
    {
        var sapRecords = new List<SapBusinessPartnerRecord>
        {
            new() { Name = "New Company", TaxNumber = "1234567890", Category = "1", SapId = "SAP-001" },
            new() { Name = "New Person", TaxNumber = "9876543210", Category = "2", SapId = "SAP-002" }
        };

        _sapClientMock.Setup(c => c.GetBusinessPartnersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sapRecords);
        _repositoryMock.Setup(r => r.GetByTaxNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BusinessPartner?)null);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<BusinessPartner>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BusinessPartner bp, CancellationToken _) => bp);

        var command = new SyncBusinessPartnersFromSapCommand();
        var result = await _handler.Handle(command, CancellationToken.None);

        result.AddedCount.Should().Be(2);
        result.UpdatedCount.Should().Be(0);
        result.Errors.Should().BeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<BusinessPartner>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ExistingRecords_ShouldUpdateInRepository()
    {
        var existingEntity = new BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            TaxNumber = "1234567890"
        };
        var sapRecords = new List<SapBusinessPartnerRecord>
        {
            new() { Name = "Updated Name", TaxNumber = "1234567890", Category = "1", SapId = "SAP-001" }
        };

        _sapClientMock.Setup(c => c.GetBusinessPartnersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sapRecords);
        _repositoryMock.Setup(r => r.GetByTaxNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);
        _repositoryMock.Setup(r => r.UpdateAsync(existingEntity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new SyncBusinessPartnersFromSapCommand(), CancellationToken.None);

        result.AddedCount.Should().Be(0);
        result.UpdatedCount.Should().Be(1);
        existingEntity.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Handle_EmptyTaxNumber_ShouldSkipAndAddToErrors()
    {
        var sapRecords = new List<SapBusinessPartnerRecord>
        {
            new() { Name = "No Tax", TaxNumber = null, Category = "1", SapId = "SAP-NOTAX" },
            new() { Name = "Empty Tax", TaxNumber = "", Category = "2", SapId = "SAP-EMPTY" },
            new() { Name = "Valid", TaxNumber = "1234567890", Category = "1", SapId = "SAP-OK" }
        };

        _sapClientMock.Setup(c => c.GetBusinessPartnersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sapRecords);
        _repositoryMock.Setup(r => r.GetByTaxNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync((BusinessPartner?)null);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<BusinessPartner>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BusinessPartner bp, CancellationToken _) => bp);

        var result = await _handler.Handle(new SyncBusinessPartnersFromSapCommand(), CancellationToken.None);

        result.AddedCount.Should().Be(1);
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().AllSatisfy(e => e.Should().Contain("TaxNumber bos"));
    }

    [Fact]
    public async Task Handle_MixedUpsert_ShouldReturnCorrectCounts()
    {
        var existing = new BusinessPartner { Id = Guid.NewGuid(), Name = "Existing", TaxNumber = "1111111111" };
        var sapRecords = new List<SapBusinessPartnerRecord>
        {
            new() { Name = "Updated", TaxNumber = "1111111111", Category = "1", SapId = "SAP-1" },
            new() { Name = "Brand New", TaxNumber = "2222222222", Category = "2", SapId = "SAP-2" },
            new() { Name = "Skip", TaxNumber = null, SapId = "SAP-3" }
        };

        _sapClientMock.Setup(c => c.GetBusinessPartnersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(sapRecords);
        _repositoryMock.Setup(r => r.GetByTaxNumberAsync("1111111111", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.GetByTaxNumberAsync("2222222222", It.IsAny<CancellationToken>())).ReturnsAsync((BusinessPartner?)null);
        _repositoryMock.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<BusinessPartner>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BusinessPartner bp, CancellationToken _) => bp);

        var result = await _handler.Handle(new SyncBusinessPartnersFromSapCommand(), CancellationToken.None);

        result.AddedCount.Should().Be(1);
        result.UpdatedCount.Should().Be(1);
        result.Errors.Should().HaveCount(1);
    }
}
