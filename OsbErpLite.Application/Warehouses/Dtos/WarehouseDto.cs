namespace OsbErpLite.Application.Warehouses.Dtos;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
