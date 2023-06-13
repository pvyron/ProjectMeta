using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProMe.Abstractions;
using ProMe.ApiContracts.Auth;
using ProMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Commands;

public sealed record Refresh(string AccessToken, string RefreshToken) : IRequest<IResult>;

public sealed class RefreshValidator : AbstractValidator<Refresh>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
        RuleFor(x => x.AccessToken).Must(x => x.ToUpper().StartsWith("BEARER "));
    }
}

public sealed class RefreshHandler : IRequestHandler<Refresh, IResult>
{
    public IAuthorizationService _authorizationService { get; }

    public RefreshHandler(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<IResult> Handle(Refresh request, CancellationToken cancellationToken)
    {
        try
        {
            var refreshedSession = await _authorizationService.RefreshSession(request.AccessToken.Remove(0, 7), request.RefreshToken);

            return Results.Ok(new LoginResponseModel
            {
                BearerToken = refreshedSession.accessToken,
                RefreshToken = refreshedSession.refreshToken,
            });
        }
        catch (AuthorizationException)
        {
            return Results.Unauthorized();
        }
    }
}
