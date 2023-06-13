using Azure.Data.Tables;
using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProMe.Abstractions;
using ProMe.ApiContracts.Auth;
using ProMe.DataAccess;
using ProMe.DataAccess.Models;
using ProMe.Workflow.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        var userData = await _proMeDB.Users.AsNoTracking().Where(u => u.Email == request.Model.Email).Select(u => new { u.Id, u.Key, u.Salt, u.Verified }).FirstAsync(cancellationToken: cancellationToken);

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