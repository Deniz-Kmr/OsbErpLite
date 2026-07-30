using FluentValidation;
using OsbErpLite.Application.BusinessPartners.Commands;

namespace OsbErpLite.Application.BusinessPartners.Validators;

public class UpdateBusinessPartnerCommandValidator : AbstractValidator<UpdateBusinessPartnerCommand>
{
    public UpdateBusinessPartnerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("İş ortağı Id'si zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("İş ortağı adı zorunludur.")
            .MaximumLength(200).WithMessage("İş ortağı adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Geçersiz iş ortağı tipi.");

        RuleFor(x => x.TaxNumber)
            .Matches(@"^\d{10,11}$").WithMessage("Vergi numarası 10 veya 11 haneli rakamlardan oluşmalıdır.")
            .When(x => !string.IsNullOrEmpty(x.TaxNumber));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("Şehir adı en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.City));
    }
}
