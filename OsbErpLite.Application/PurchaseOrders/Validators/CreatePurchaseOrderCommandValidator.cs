using FluentValidation;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.PurchaseOrders.Commands;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.PurchaseOrders.Validators;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator(
        IBusinessPartnerRepository businessPartnerRepository,
        IProductRepository productRepository)
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Tedarikci ID zorunludur.")
            .MustAsync(async (supplierId, ct) =>
            {
                var partner = await businessPartnerRepository.GetByIdAsync(supplierId, ct);
                return partner is not null &&
                       (partner.Type == BusinessPartnerType.Supplier || partner.Type == BusinessPartnerType.Both);
            }).WithMessage("Belirtilen ID'ye ait bir tedarikci (Supplier veya Both) bulunamadi.");

        RuleFor(x => x.OrderDate)
            .NotEmpty().WithMessage("Siparis tarihi zorunludur.");

        RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("En az bir siparis satiri olmalidir.")
            .Must(lines => lines.Count > 0).WithMessage("En az bir siparis satiri olmalidir.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId)
                .NotEmpty().WithMessage("Urun ID zorunludur.")
                .MustAsync(async (productId, ct) =>
                {
                    var product = await productRepository.GetByIdAsync(productId, ct);
                    return product is not null;
                }).WithMessage("Belirtilen ID'ye ait urun bulunamadi.");

            line.RuleFor(l => l.Quantity)
                .GreaterThan(0).WithMessage("Miktar sifirdan buyuk olmalidir.");

            line.RuleFor(l => l.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Birim fiyat negatif olamaz.");
        });
    }
}
