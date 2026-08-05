using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.Departments.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.HumanResources.Departments.Commands;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;

    public CreateDepartmentCommandHandler(IDepartmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Department>(request);
        var created = await _repository.AddAsync(entity, cancellationToken);
        return _mapper.Map<DepartmentDto>(created);
    }
}
