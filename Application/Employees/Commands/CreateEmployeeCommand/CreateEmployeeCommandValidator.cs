using FluentValidation;

namespace Application.Employees.Commands.CreateEmployeeCommand;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Position).NotEmpty();
    }
}