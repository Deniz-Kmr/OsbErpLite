namespace OsbErpLite.Application.HumanResources.Employees.Dtos;

public class CreateEmployeeDto
{
    public string FullName { get; set; } = default!;
    public string NationalId { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public Guid? ManagerId { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
}
