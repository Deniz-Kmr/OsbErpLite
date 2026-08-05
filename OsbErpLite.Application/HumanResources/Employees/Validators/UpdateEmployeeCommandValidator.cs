using FluentValidation;
using OsbErpLite.Application.HumanResources.Employees.Commands;

namespace OsbErpLite.Application.HumanResources.Employees.Validators;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Calisan Id zorunludur.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Calisan adi zorunludur.")
            .MaximumLength(200).WithMessage("Calisan adi en fazla 200 karakter olabilir.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("TC Kimlik No zorunludur.")
            .Length(11).WithMessage("TC Kimlik No 11 haneli olmalidir.")
            .Matches(@"^\d{11}$").WithMessage("TC Kimlik No sadece rakamlardan olusmalıdir.")
            .Must(BeValidTcKimlikNo).WithMessage("Gecersiz TC Kimlik No.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Departman secimi zorunludur.");

        RuleFor(x => x.PositionId)
            .NotEmpty().WithMessage("Pozisyon secimi zorunludur.");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Ise baslama tarihi zorunludur.");

        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("Maas 0'dan buyuk olmalidir.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Gecersiz calisan durumu.");

        // ManagerId kendisiyle ayni olamaz
        RuleFor(x => x)
            .Must(x => x.ManagerId != x.Id)
            .WithMessage("Bir calisan kendi yoneticisi olamaz.")
            .When(x => x.ManagerId.HasValue);
    }

    private static bool BeValidTcKimlikNo(string nationalId)
    {
        if (string.IsNullOrEmpty(nationalId) || nationalId.Length != 11)
            return false;

        if (nationalId[0] == '0')
            return false;

        var digits = new int[11];
        for (int i = 0; i < 11; i++)
        {
            if (!char.IsDigit(nationalId[i]))
                return false;
            digits[i] = nationalId[i] - '0';
        }

        int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        int evenSum = digits[1] + digits[3] + digits[5] + digits[7];
        if ((oddSum * 7 - evenSum) % 10 != digits[9])
            return false;

        int totalSum = 0;
        for (int i = 0; i < 10; i++)
            totalSum += digits[i];
        if (totalSum % 10 != digits[10])
            return false;

        return true;
    }
}
