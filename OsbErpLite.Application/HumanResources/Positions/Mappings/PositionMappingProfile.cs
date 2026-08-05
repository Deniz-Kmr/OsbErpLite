using AutoMapper;
using OsbErpLite.Application.HumanResources.Positions.Commands;
using OsbErpLite.Application.HumanResources.Positions.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.HumanResources.Positions.Mappings;

public class PositionMappingProfile : Profile
{
    public PositionMappingProfile()
    {
        CreateMap<Position, PositionDto>();

        CreateMap<CreatePositionCommand, Position>();

        CreateMap<UpdatePositionCommand, Position>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
