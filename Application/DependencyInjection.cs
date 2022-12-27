using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using Application.Common.Behaviours;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(executingAssembly);
        services.AddValidatorsFromAssembly(executingAssembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

        return services;
    }
}