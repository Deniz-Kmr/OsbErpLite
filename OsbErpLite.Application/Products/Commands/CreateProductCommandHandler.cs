using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Products.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Product>(request);
        var created = await _repository.AddAsync(entity, cancellationToken);

        // Include etmek icin tekrar cek
        var loaded = await _repository.GetByIdAsync(created.Id, cancellationToken);
        return _mapper.Map<ProductDto>(loaded);
    }
}
