using FluentValidation;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.SalesOrders.Commands;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.SalesOrders.Validators;

public class CreateSalesOrderCommandValidator : AbstractValidator<CreateSalesOrderCommand>
{
    public CreateSalesOrderCommandValidator(
        IBusinessPartnerRepository businessPartnerRepository,
        IProductRepository productRepository)
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Musteri ID zorunludur.")
            .MustAsync(async (customerId, ct) =>
            {
                var partner = await businessPartnerRepository.GetByIdAsync(customerId, ct);
                return partner is not null &&
                       (partner.Type == BusinessPartnerType.Customer || partner.Type == BusinessPartnerType.Both);
            }).WithMessage("Belirtilen ID'ye ait bir musteri (Customer veya Both) bulunamadi.");

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
