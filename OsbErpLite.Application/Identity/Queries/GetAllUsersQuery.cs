using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Queries;

public record GetAllUsersQuery(int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize) : IRequest<PaginatedList<UserDto>>;
