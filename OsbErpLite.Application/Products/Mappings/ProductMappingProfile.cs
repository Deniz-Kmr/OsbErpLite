using AutoMapper;
using OsbErpLite.Application.Products.Commands;
using OsbErpLite.Application.Products.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Products.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name));

        CreateMap<CreateProductCommand, Product>();

        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StockQuantity, opt => opt.Ignore());
    }
}
