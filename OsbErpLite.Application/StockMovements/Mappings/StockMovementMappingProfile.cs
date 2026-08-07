using AutoMapper;
using OsbErpLite.Application.StockMovements.Commands;
using OsbErpLite.Application.StockMovements.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.StockMovements.Mappings;

public class StockMovementMappingProfile : Profile
{
    public StockMovementMappingProfile()
    {
        CreateMap<StockMovement, StockMovementDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.MovementTypeName, opt => opt.MapFrom(src => src.MovementType.ToString()));

        CreateMap<CreateStockMovementCommand, StockMovement>();
    }
}
