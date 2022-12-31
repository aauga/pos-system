using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Customers.Commands.CreateCustomerCommand;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    private readonly IApplicationDbContext _dbContext;
    
    public CreateCustomerCommandValidator(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();

        // Email and phone number cannot exist already in a tenant
        RuleFor(x => x.Email)
            .EmailAddress()
            .Must((request, email) =>
                !EmailExists(request.TenantId, email))
            .WithMessage("Email already exists in tenant");

        RuleFor(x => x.PhoneNumber)
            .Must((request, phoneNumber) =>
                !PhoneNumberExists(request.TenantId, phoneNumber))
            .WithMessage("Phone number already exists in tenant");

        // Only one field is required
        RuleFor(x => x.Email)
            .NotEmpty()
            .When(x => string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Either email or phone number must be entered");
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .When(x => string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Either email or phone number must be entered");
    }

    private bool EmailExists(int tenantId, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return _dbContext.Customers.Any(x =>
            x.TenantId == tenantId &&
            x.Email == email);
    }

    private bool PhoneNumberExists(int tenantId, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        return _dbContext.Customers.Any(x =>
            x.TenantId == tenantId &&
            x.PhoneNumber == phoneNumber);
    }
}