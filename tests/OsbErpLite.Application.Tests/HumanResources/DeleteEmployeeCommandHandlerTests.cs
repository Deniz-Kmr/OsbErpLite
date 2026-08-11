using FluentAssertions;
using MediatR;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.Employees.Commands;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Tests.HumanResources;

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;
    private readonly Mock<ILeaveRequestRepository> _leaveRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _employeeRepoMock = new Mock<IEmployeeRepository>();
        _leaveRepoMock = new Mock<ILeaveRequestRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _handler = new DeleteEmployeeCommandHandler(
            _employeeRepoMock.Object, _leaveRepoMock.Object, _departmentRepoMock.Object);
    }

    [Fact]
    public async Task Handle_NoBlockers_ShouldSoftDelete()
    {
        var empId = Guid.NewGuid();
        var employee = new Employee { Id = empId, FullName = "Test", IsActive = true };

        _employeeRepoMock.Setup(r => r.GetByIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _employeeRepoMock.Setup(r => r.CountActiveByManagerIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _leaveRepoMock.Setup(r => r.CountPendingByEmployeeIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _departmentRepoMock.Setup(r => r.IsManagerOfAnyDepartmentAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _employeeRepoMock.Setup(r => r.UpdateAsync(employee, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new DeleteEmployeeCommand(empId), CancellationToken.None);

        employee.IsActive.Should().BeFalse();
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_WithActiveSubordinates_ShouldThrowInvalidOperation()
    {
        var empId = Guid.NewGuid();
        var employee = new Employee { Id = empId, FullName = "Manager", IsActive = true };

        _employeeRepoMock.Setup(r => r.GetByIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _employeeRepoMock.Setup(r => r.CountActiveByManagerIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(3);

        var act = () => _handler.Handle(new DeleteEmployeeCommand(empId), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*3 alt calisan*");
    }

    [Fact]
    public async Task Handle_WithPendingLeaveRequests_ShouldThrowInvalidOperation()
    {
        var empId = Guid.NewGuid();
        var employee = new Employee { Id = empId, FullName = "LeaveTest", IsActive = true };

        _employeeRepoMock.Setup(r => r.GetByIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _employeeRepoMock.Setup(r => r.CountActiveByManagerIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _leaveRepoMock.Setup(r => r.CountPendingByEmployeeIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(2);

        var act = () => _handler.Handle(new DeleteEmployeeCommand(empId), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Bekleyen izin talebi*");
    }

    [Fact]
    public async Task Handle_IsDepartmentManager_ShouldThrowInvalidOperation()
    {
        var empId = Guid.NewGuid();
        var employee = new Employee { Id = empId, FullName = "DeptManager", IsActive = true };

        _employeeRepoMock.Setup(r => r.GetByIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _employeeRepoMock.Setup(r => r.CountActiveByManagerIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _leaveRepoMock.Setup(r => r.CountPendingByEmployeeIdAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _departmentRepoMock.Setup(r => r.IsManagerOfAnyDepartmentAsync(empId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var act = () => _handler.Handle(new DeleteEmployeeCommand(empId), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*departmanin yoneticisi*");
    }

    [Fact]
    public async Task Handle_NonExistentEmployee_ShouldThrowKeyNotFound()
    {
        var empId = Guid.NewGuid();
        _employeeRepoMock.Setup(r => r.GetByIdAsync(empId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var act = () => _handler.Handle(new DeleteEmployeeCommand(empId), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
