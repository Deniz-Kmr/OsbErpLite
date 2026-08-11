using System.Text.Json.Serialization;

namespace OsbErpLite.Infrastructure.ExternalServices.Sap;

public class SapODataResponse<T>
{
    [JsonPropertyName("d")]
    public SapODataResultWrapper<T> D { get; set; } = default!;
}

public class SapODataResultWrapper<T>
{
    [JsonPropertyName("results")]
    public List<T> Results { get; set; } = new();
}
