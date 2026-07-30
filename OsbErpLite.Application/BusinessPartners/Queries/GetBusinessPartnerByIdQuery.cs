using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;

namespace OsbErpLite.Application.BusinessPartners.Queries;

public record GetBusinessPartnerByIdQuery(Guid Id) : IRequest<BusinessPartnerDto?>;
