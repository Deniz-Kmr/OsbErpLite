using AutoMapper;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Identity.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
