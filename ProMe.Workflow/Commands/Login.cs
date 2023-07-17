﻿using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProMe.Abstractions;
using ProMe.ApiContracts.Auth;
using ProMe.DataAccess;

namespace ProMe.Workflow.Commands;
public record Login(LoginRequestModel Model) : IRequest<IResult>;

public sealed class LoginValidator : AbstractValidator<Login>
{
    public LoginValidator()
    {
        RuleFor(x => x.Model.Email).EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
        RuleFor(x => x.Model.Password).Length(6, 32);
    }
}

internal sealed class LoginHandler : IRequestHandler<Login, IResult>
{
    private readonly ProMeDBContext _proMeDB;
    private readonly IAuthenticationService _authenticationService;
    private readonly IAuthorizationService _authorizationService;

    public LoginHandler(ProMeDBContext proMeDB, IAuthenticationService authenticationService, IAuthorizationService authorizationService)
    {
        _proMeDB = proMeDB;
        _authenticationService = authenticationService;
        _authorizationService = authorizationService;

    }

    public async Task<IResult> Handle(Login request, CancellationToken cancellationToken)
    {
        var userData = await _proMeDB.Users.AsNoTracking().Where(u => u.Email == request.Model.Email).Select(u => new { u.Id, u.Key, u.Salt, u.Verified }).FirstOrDefaultAsync(cancellationToken);

        if (userData is null || !userData.Verified)
        {
            return Results.Unauthorized();
        }

        var verified = await _authenticationService.AuthenticatePasswordForEmail(request.Model.Password, userData.Key, userData.Salt);

        if (!verified)
        {
            return Results.Unauthorized();
        }

        var bearer = await _authorizationService.GenerateAccessTokenForUser(userData.Id);
        var refresh = await _authorizationService.GenerateRefreshTokenForUser(userData.Id);

        return Results.Ok(new LoginResponseModel
        {
            BearerToken = bearer,
            RefreshToken = refresh,
        });
    }
}