using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Queries;

public record GetAllUsersQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedList<UserDto>>;
