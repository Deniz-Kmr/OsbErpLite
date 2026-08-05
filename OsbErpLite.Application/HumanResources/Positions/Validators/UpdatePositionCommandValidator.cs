using FluentValidation;
using OsbErpLite.Application.HumanResources.Positions.Commands;

namespace OsbErpLite.Application.HumanResources.Positions.Validators;

public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Pozisyon Id zorunludur.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Pozisyon basligi zorunludur.")
            .MaximumLength(200).WithMessage("Pozisyon basligi en fazla 200 karakter olabilir.");

        RuleFor(x => x.Level)
            .GreaterThan(0).WithMessage("Pozisyon seviyesi 0'dan buyuk olmalidir.");
    }
}
