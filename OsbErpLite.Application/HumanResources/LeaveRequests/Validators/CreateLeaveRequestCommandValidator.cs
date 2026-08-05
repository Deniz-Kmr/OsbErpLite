using FluentValidation;
using OsbErpLite.Application.HumanResources.LeaveRequests.Commands;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Validators;

public class CreateLeaveRequestCommandValidator : AbstractValidator<CreateLeaveRequestCommand>
{
    public CreateLeaveRequestCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Calisan secimi zorunludur.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Baslangic tarihi zorunludur.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Gecmis tarihli izin talebi olusturulamaz.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitis tarihi zorunludur.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Bitis tarihi baslangic tarihinden once olamaz.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Izin sebebi en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}
