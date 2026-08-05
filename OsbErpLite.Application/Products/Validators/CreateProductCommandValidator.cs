using FluentValidation;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Products.Commands;

namespace OsbErpLite.Application.Products.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IProductRepository productRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Urun adi zorunludur.")
            .MaximumLength(200).WithMessage("Urun adi en fazla 200 karakter olabilir.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU zorunludur.")
            .MaximumLength(50).WithMessage("SKU en fazla 50 karakter olabilir.")
            .MustAsync(async (sku, ct) => !await productRepository.ExistsBySkuAsync(sku, null, ct))
            .WithMessage("Bu SKU ile kayitli bir urun zaten mevcut.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Birim fiyat negatif olamaz.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktari negatif olamaz.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Depo secimi zorunludur.");
    }
}
