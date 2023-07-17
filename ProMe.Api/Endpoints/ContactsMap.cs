using MediatR;
using ProMe.ApiContracts.Contacts;
using ProMe.Workflow.Commands;
using ProMe.Workflow.Filters;
using ProMe.Workflow.Models;
using ProMe.Workflow.Queries;

namespace ProMe.Api.Endpoints;

public static class ContactsMap
{
    public static WebApplication MapContacts(this WebApplication app)
    {
        app.MapGet("/Contacts", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new GetAllContacts(), cancellationToken);
        }).AddEndpointFilter<BearerAuthorizationFilter>();

        app.MapGet("/Contacts/{id:Guid}", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new GetContact(id), cancellationToken);
        }).AddEndpointFilter<BearerAuthorizationFilter>();

        app.MapPost("/Contacts", async (IMediator mediator, ContactRequestModel requestModel, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new AddContact(requestModel), cancellationToken);
        }).AddEndpointFilter<BearerAuthorizationFilter>();

        app.MapPut("/Contacts/{id:Guid}", async (IMediator mediator, Guid id, ContactRequestModel requestModel, CancellationToken cancellationToekn) =>
        {
            return await mediator.Send(new UpdateContact(id, requestModel), cancellationToekn);
        }).AddEndpointFilter<BearerAuthorizationFilter>();

        return app;
    }
}
