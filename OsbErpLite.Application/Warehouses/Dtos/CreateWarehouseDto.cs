namespace OsbErpLite.Application.Warehouses.Dtos;

public class CreateWarehouseDto
{
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
}
