using FluentValidation;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Products.Commands;

namespace OsbErpLite.Application.Products.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(IProductRepository productRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Urun Id zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Urun adi zorunludur.")
            .MaximumLength(200).WithMessage("Urun adi en fazla 200 karakter olabilir.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU zorunludur.")
            .MaximumLength(50).WithMessage("SKU en fazla 50 karakter olabilir.")
            .MustAsync(async (command, sku, ct) => !await productRepository.ExistsBySkuAsync(sku, command.Id, ct))
            .WithMessage("Bu SKU ile kayitli bir urun zaten mevcut.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Birim fiyat negatif olamaz.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Depo secimi zorunludur.");
    }
}
