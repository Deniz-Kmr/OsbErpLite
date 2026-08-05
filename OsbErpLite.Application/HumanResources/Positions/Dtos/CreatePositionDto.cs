namespace OsbErpLite.Application.HumanResources.Positions.Dtos;

public class CreatePositionDto
{
    public string Title { get; set; } = default!;
    public int Level { get; set; }
}
