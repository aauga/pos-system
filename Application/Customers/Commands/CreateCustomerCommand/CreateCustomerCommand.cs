using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Customers.Dtos;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Customers.Commands.CreateCustomerCommand;

public record CreateCustomerCommand : IAuthorizedRequest<CustomerDto>
{
    public int TenantId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }

    public async Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        return await userService.CanAccessTenantAsync(employee, TenantId);
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateCustomerCommandHandler(
        IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var id = _dbContext.Customers.Any() ? _dbContext.Customers.Max(x => x.Id + 1) : 1;
        
        var customer = new Customer
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            TenantId = request.TenantId,
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        return new CustomerDto(customer);
    }
}