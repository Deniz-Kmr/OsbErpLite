using FluentValidation;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Warehouses.Commands;

namespace OsbErpLite.Application.Warehouses.Validators;

public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator(IWarehouseRepository warehouseRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Depo Id zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Depo adi zorunludur.")
            .MaximumLength(200).WithMessage("Depo adi en fazla 200 karakter olabilir.")
            .MustAsync(async (command, name, ct) => !await warehouseRepository.ExistsByNameAsync(name, command.Id, ct))
            .WithMessage("Bu isimde bir depo zaten mevcut.");

        RuleFor(x => x.Location)
            .MaximumLength(500).WithMessage("Konum en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Location));
    }
}
