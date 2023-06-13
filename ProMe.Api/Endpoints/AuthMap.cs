using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProMe.ApiContracts.Auth;
using ProMe.Workflow.Commands;
using ProMe.Workflow.Queries;

namespace ProMe.Api.Endpoints;

public static class AuthMap
{
    public static WebApplication MapAuth(this WebApplication app)
    {
        app.MapPost("/Register", async (IMediator mediator, RegisterRequestModel requestModel, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new Register(requestModel), cancellationToken);
        });

        app.MapPost("/Login", async (IMediator mediator, LoginRequestModel requestModel, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new Login(requestModel), cancellationToken);
        });

        app.MapGet("/Refresh", async (IMediator mediator, [FromHeader(Name = "AccessToken")] string accessToken, [FromHeader(Name = "Refresh")] string refreshToken, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new Refresh(accessToken, refreshToken), cancellationToken);
        });

        app.MapGet("/VerifyEmail/{verificationSoup}", async (IMediator mediator, string verificationSoup, CancellationToken cancellationToken) =>
        {
            return await mediator.Send(new VerifyEmail(verificationSoup), cancellationToken);
        });

        return app;
    }
}
