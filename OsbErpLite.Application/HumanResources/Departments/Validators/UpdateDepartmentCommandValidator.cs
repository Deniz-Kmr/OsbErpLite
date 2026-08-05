using FluentValidation;
using OsbErpLite.Application.HumanResources.Departments.Commands;

namespace OsbErpLite.Application.HumanResources.Departments.Validators;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Departman Id zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Departman adi zorunludur.")
            .MaximumLength(200).WithMessage("Departman adi en fazla 200 karakter olabilir.");
    }
}
