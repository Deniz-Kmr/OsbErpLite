namespace OsbErpLite.Application.HumanResources.Positions.Dtos;

public class PositionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int Level { get; set; }
    public DateTime CreatedDate { get; set; }
}
