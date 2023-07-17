using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProMe.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Filters;
public class AdminKeyAthorizationFilter : IEndpointFilter
{
    private readonly IConfiguration _configuration;

    public AdminKeyAthorizationFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("x-api-key", out var apiKeys))
        {
            return Results.Unauthorized();
        }

        if (apiKeys.Count != 1)
        {
            return Results.Unauthorized();
        }

        if (apiKeys[0] != _configuration.GetValue<string>("Authorization:AdminKey"))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}
