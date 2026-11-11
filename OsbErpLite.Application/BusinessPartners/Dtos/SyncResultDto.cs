namespace OsbErpLite.Application.BusinessPartners.Dtos;

public class SyncResultDto
{
    public int AddedCount { get; set; }
    public int UpdatedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
