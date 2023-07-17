using MediatR;
using Microsoft.IdentityModel.Protocols;
using ProMe.ApiContracts.Campaign;
using ProMe.Workflow.Commands;
using ProMe.Workflow.Filters;
using System.Reflection.Metadata.Ecma335;

namespace ProMe.Api.Endpoints;

public static class CampaignsMap
{
    public static WebApplication MapCampaigns(this WebApplication app)
    {
        app.MapPost("/CampaingManagers", async (IMediator mediator, CampaignManagerRequestModel requestModel, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new CreateCampaignManager(requestModel), cancellationToken);
        }).AddEndpointFilter<AdminKeyAthorizationFilter>();

        return app;
    }
}
