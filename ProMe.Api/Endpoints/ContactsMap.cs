using MediatR;
using ProMe.Workflow.Commands;
using ProMe.Workflow.Filters;

namespace ProMe.Api.Endpoints;

public static class ContactsMap
{
    public static WebApplication MapContacts(this WebApplication app)
    {
        app.MapPost("/contacts", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new AddContact("Nick the greek from spata"), cancellationToken);
        }).AddEndpointFilter<BearerAuthorizationFilter>();

        return app;
    }
}
