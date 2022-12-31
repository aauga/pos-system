using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Tenants.Dtos;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Tenants.Commands.CreateTenantCommand;

public record CreateTenantCommand : IAuthorizedRequest<TenantDto>
{
    public string Name { get; init; }
    public DateTime ActiveFrom { get; init; }
    public DateTime ActiveTo { get; init; }

    public Task<bool> Authorize(Employee employee, IUserService userService, IApplicationDbContext dbContext)
    {
        var authorized = employee.Position == PositionType.Admin;
        return Task.FromResult(authorized);
    }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateTenantCommandHandler(
        IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var id = _dbContext.Tenants.Any() ? _dbContext.Tenants.Max(x => x.Id + 1) : 1;
        
        var tenant = new Tenant
        {
            Id = id,
            Name = request.Name,
            ActiveFrom = request.ActiveFrom,
            ActiveTo = request.ActiveTo
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        return new TenantDto(tenant);
    }
}