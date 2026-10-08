using FluentAssertions;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Dashboard.Dtos;
using OsbErpLite.Application.Dashboard.Queries;

namespace OsbErpLite.Application.Tests.Dashboard;

public class GetDepartmentEmployeeDistributionQueryHandlerTests
{
    private readonly Mock<IDashboardRepository> _repositoryMock;
    private readonly GetDepartmentEmployeeDistributionQueryHandler _handler;

    public GetDepartmentEmployeeDistributionQueryHandlerTests()
    {
        _repositoryMock = new Mock<IDashboardRepository>();
        _handler = new GetDepartmentEmployeeDistributionQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDistribution()
    {
        var data = new List<DepartmentDistributionDto>
        {
            new() { DepartmentName = "IT", EmployeeCount = 15 },
            new() { DepartmentName = "HR", EmployeeCount = 5 }
        };

        _repositoryMock.Setup(r => r.GetDepartmentEmployeeDistributionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _handler.Handle(new GetDepartmentEmployeeDistributionQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].DepartmentName.Should().Be("IT");
        result[0].EmployeeCount.Should().Be(15);
    }

    [Fact]
    public async Task Handle_NoDepartments_ShouldReturnEmptyList()
    {
        _repositoryMock.Setup(r => r.GetDepartmentEmployeeDistributionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentDistributionDto>());

        var result = await _handler.Handle(new GetDepartmentEmployeeDistributionQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
