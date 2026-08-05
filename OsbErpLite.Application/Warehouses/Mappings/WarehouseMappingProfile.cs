using AutoMapper;
using OsbErpLite.Application.Warehouses.Commands;
using OsbErpLite.Application.Warehouses.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Warehouses.Mappings;

public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseCommand, Warehouse>();
        CreateMap<UpdateWarehouseCommand, Warehouse>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
