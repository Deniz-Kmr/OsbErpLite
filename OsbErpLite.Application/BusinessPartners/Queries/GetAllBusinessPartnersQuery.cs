using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Models;

namespace OsbErpLite.Application.BusinessPartners.Queries;

public record GetAllBusinessPartnersQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedList<BusinessPartnerDto>>;
