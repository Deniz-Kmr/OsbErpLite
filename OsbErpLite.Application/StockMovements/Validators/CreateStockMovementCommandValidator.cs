using FluentValidation;
using OsbErpLite.Application.StockMovements.Commands;

namespace OsbErpLite.Application.StockMovements.Validators;

public class CreateStockMovementCommandValidator : AbstractValidator<CreateStockMovementCommand>
{
    public CreateStockMovementCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Urun secimi zorunludur.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miktar sifirdan buyuk olmalidir.");

        RuleFor(x => x.MovementType)
            .IsInEnum().WithMessage("Gecersiz hareket tipi.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Not en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Note));
    }
}
