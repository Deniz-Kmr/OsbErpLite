using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Models;

namespace OsbErpLite.Application.BusinessPartners.Queries;

public record GetAllBusinessPartnersQuery(int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize) : IRequest<PaginatedList<BusinessPartnerDto>>;
