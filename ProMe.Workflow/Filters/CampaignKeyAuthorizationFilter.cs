using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProMe.Abstractions;
using ProMe.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Filters;
public class CampaignKeyAuthorizationFilter : IEndpointFilter
{
    private readonly ProMeDBContext _proMeDB;
    private readonly IIdentityProvider _identityProvider;

    public CampaignKeyAuthorizationFilter(ProMeDBContext proMeDB, IIdentityProvider identityProvider)
    {
        _proMeDB = proMeDB;
        _identityProvider = identityProvider;
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

        var campaignId = await _proMeDB.CampaignManagers.AsNoTracking().FirstOrDefaultAsync(cm => cm.AccessKey == apiKeys[0]).Select(cm => cm?.Id);

        if (campaignId is null)
        {
            return Results.Unauthorized();
        }

        _identityProvider.AddClaim(new Claim("CampaignManagerId", campaignId.ToString()!));

        return await next(context);
    }
}
