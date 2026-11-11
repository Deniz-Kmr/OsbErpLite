using FluentAssertions;
using MediatR;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.Departments.Commands;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Tests.HumanResources;

public class DeleteDepartmentCommandHandlerTests
{
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;
    private readonly DeleteDepartmentCommandHandler _handler;

    public DeleteDepartmentCommandHandlerTests()
    {
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _employeeRepoMock = new Mock<IEmployeeRepository>();
        _handler = new DeleteDepartmentCommandHandler(_departmentRepoMock.Object, _employeeRepoMock.Object);
    }

    [Fact]
    public async Task Handle_NoActiveEmployees_ShouldSoftDelete()
    {
        var deptId = Guid.NewGuid();
        var department = new Department { Id = deptId, Name = "IT", IsActive = true };

        _departmentRepoMock.Setup(r => r.GetByIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(department);
        _employeeRepoMock.Setup(r => r.CountActiveByDepartmentIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _departmentRepoMock.Setup(r => r.UpdateAsync(department, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new DeleteDepartmentCommand(deptId), CancellationToken.None);

        department.IsActive.Should().BeFalse();
        result.Should().Be(Unit.Value);
        _departmentRepoMock.Verify(r => r.UpdateAsync(department, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithActiveEmployees_ShouldThrowInvalidOperation()
    {
        var deptId = Guid.NewGuid();
        var department = new Department { Id = deptId, Name = "HR", IsActive = true };

        _departmentRepoMock.Setup(r => r.GetByIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(department);
        _employeeRepoMock.Setup(r => r.CountActiveByDepartmentIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var act = () => _handler.Handle(new DeleteDepartmentCommand(deptId), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*5 aktif calisan*");
    }

    [Fact]
    public async Task Handle_NonExistentDepartment_ShouldThrowKeyNotFound()
    {
        var deptId = Guid.NewGuid();
        _departmentRepoMock.Setup(r => r.GetByIdAsync(deptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Department?)null);

        var act = () => _handler.Handle(new DeleteDepartmentCommand(deptId), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
