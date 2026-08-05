namespace OsbErpLite.Application.HumanResources.Departments.Dtos;

public class UpdateDepartmentDto
{
    public string Name { get; set; } = default!;
    public Guid? ManagerId { get; set; }
}
