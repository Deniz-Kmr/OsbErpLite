using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.HumanResources.Employees.Dtos;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string NationalId { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = default!;
    public Guid PositionId { get; set; }
    public string PositionTitle { get; set; } = default!;
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; }
    public string StatusName { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
}
