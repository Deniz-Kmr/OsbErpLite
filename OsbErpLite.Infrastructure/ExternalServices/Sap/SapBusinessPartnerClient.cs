using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Infrastructure.ExternalServices.Sap;

public class SapBusinessPartnerClient : ISapBusinessPartnerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SapBusinessPartnerClient> _logger;

    public SapBusinessPartnerClient(HttpClient httpClient, ILogger<SapBusinessPartnerClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<SapBusinessPartnerRecord>> GetBusinessPartnersAsync(CancellationToken cancellationToken = default)
    {
        var selectFields = "BusinessPartner,BusinessPartnerFullName,BusinessPartnerName,BusinessPartnerCategory,TaxNumber1,TaxNumber2,SearchTerm1,BusinessPartnerGrouping";
        var requestUri = $"A_BusinessPartner?$format=json&$select={selectFields}";

        _logger.LogInformation("SAP Business Partner servisi cagriliyor. URI: {RequestUri}", requestUri);

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var sapResponse = await response.Content.ReadFromJsonAsync<SapODataResponse<SapBusinessPartnerDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = false },
            cancellationToken);

        if (sapResponse?.D?.Results is null)
        {
            _logger.LogWarning("SAP'den bos veya gecersiz response dondu.");
            return new List<SapBusinessPartnerRecord>();
        }

        _logger.LogInformation("SAP'den {Count} adet Business Partner alindi.", sapResponse.D.Results.Count);

        return sapResponse.D.Results.Select(dto => new SapBusinessPartnerRecord
        {
            SapId = dto.BusinessPartner,
            Name = dto.BusinessPartnerFullName ?? dto.BusinessPartnerName,
            TaxNumber = dto.TaxNumber1 ?? dto.TaxNumber2,
            Category = dto.BusinessPartnerCategory,
            SearchTerm = dto.SearchTerm1
        }).ToList();
    }
}
