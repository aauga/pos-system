using FluentValidation;

namespace Application.Tenants.Commands.CreateTenantCommand;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ActiveFrom).NotEmpty();
        RuleFor(x => x.ActiveTo).NotEmpty().GreaterThan(x => x.ActiveFrom);
    }
}