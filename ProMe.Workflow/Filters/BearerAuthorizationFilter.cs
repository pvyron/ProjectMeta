using Microsoft.AspNetCore.Http;
using ProMe.Abstractions;
using System.Security.Claims;

namespace ProMe.Workflow.Filters;
public class BearerAuthorizationFilter : IEndpointFilter
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IIdentityProvider _identityProvider;

    public BearerAuthorizationFilter(IAuthorizationService authorizationService, IIdentityProvider identityProvider)
    {
        _authorizationService = authorizationService;
        _identityProvider = identityProvider;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var authBearerHeader = context.HttpContext.Request.Headers.Authorization;

        if (authBearerHeader.Count != 1)
        {
            return Results.Unauthorized();
        }

        var authToken = authBearerHeader[0]!;

        if (!authToken.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Unauthorized();
        }

        var userId = await _authorizationService.ValidateAccessTokenForUser(authToken.Remove(0, 7));

        if (userId is null)
        {
            return Results.Unauthorized();
        }

        _identityProvider.AddClaim(new Claim("UserId", userId.ToString()!));

        return await next(context);
    }
}

public enum Pew
{
    alo,
    pew,
    re
}