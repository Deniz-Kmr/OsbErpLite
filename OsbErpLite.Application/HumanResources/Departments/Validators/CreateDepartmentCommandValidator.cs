using FluentValidation;
using OsbErpLite.Application.HumanResources.Departments.Commands;

namespace OsbErpLite.Application.HumanResources.Departments.Validators;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Departman adi zorunludur.")
            .MaximumLength(200).WithMessage("Departman adi en fazla 200 karakter olabilir.");
    }
}
