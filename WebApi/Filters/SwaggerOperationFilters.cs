using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Filters;

public class SwaggerOperationFilters : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        Created_PostRequests(ref operation, context);
        BadRequestException_PostAndPutRequests(ref operation, context);
        UnauthorizedAndForbidden_AuthorizedRequests(ref operation, context);
        NotFoundException_PathParametersExist(ref operation, context);
    }

    private void Created_PostRequests(ref OpenApiOperation operation, OperationFilterContext context)
    {
        var isPostRequest = context.ApiDescription.HttpMethod == "POST";

        if (isPostRequest)
        {
            var success = operation.Responses.First(x => x.Key == "200");
            success.Value.Description = "Created";

            operation.Responses.Add("201", success.Value);
            operation.Responses.Remove("200");
        }
    }

    private void BadRequestException_PostAndPutRequests(ref OpenApiOperation operation, OperationFilterContext context)
    {
        var isPostOrPutRequest = context.ApiDescription.HttpMethod == "POST" ||
            context.ApiDescription.HttpMethod == "PUT";

        if (isPostOrPutRequest)
        {
            operation.Responses.Add("400", new OpenApiResponse { Description = "Validation Errors" });
        }
    }

    public void UnauthorizedAndForbidden_AuthorizedRequests(ref OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorizeAttribute = 
          context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() 
          || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorizeAttribute)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }

    private void NotFoundException_PathParametersExist(ref OpenApiOperation operation, OperationFilterContext context)
    {
        var hasPathParameters = 
          context.ApiDescription.ActionDescriptor.Parameters.Any(x => x.BindingInfo.BindingSource.Id == "Path");

        if (hasPathParameters)
        {
            operation.Responses.Add("404", new OpenApiResponse { Description = "Resource Not Found" });
        }
    }
}