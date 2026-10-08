using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Warehouses.Dtos;

namespace OsbErpLite.Application.Warehouses.Queries;

public record GetAllWarehousesQuery(int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize) : IRequest<PaginatedList<WarehouseDto>>;
