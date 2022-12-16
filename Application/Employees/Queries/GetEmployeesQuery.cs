using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Employees;

public class GetEmployeeQuery : IRequest<Employee>
{
    public Guid Id { get; init; }
}

public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, Employee>
{
    private readonly IApplicationDbContext _dbContext;

    public GetEmployeeQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Employee> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees.FindAsync(request.Id) ??
            throw new InvalidOperationException("Employee not found.");
        
        return employee;
    }
}