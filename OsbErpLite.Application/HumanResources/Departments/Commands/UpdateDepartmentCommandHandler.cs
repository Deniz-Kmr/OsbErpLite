using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.Departments.Dtos;

namespace OsbErpLite.Application.HumanResources.Departments.Commands;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;

    public UpdateDepartmentCommandHandler(IDepartmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Department with Id '{request.Id}' was not found.");

        _mapper.Map(request, entity);
        await _repository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<DepartmentDto>(entity);
    }
}
