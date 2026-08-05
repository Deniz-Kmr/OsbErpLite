using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Warehouses.Dtos;

namespace OsbErpLite.Application.Warehouses.Queries;

public record GetAllWarehousesQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedList<WarehouseDto>>;
