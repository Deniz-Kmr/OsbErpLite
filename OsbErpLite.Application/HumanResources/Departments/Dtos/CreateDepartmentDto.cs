namespace OsbErpLite.Application.HumanResources.Departments.Dtos;

public class CreateDepartmentDto
{
    public string Name { get; set; } = default!;
    public Guid? ManagerId { get; set; }
}
