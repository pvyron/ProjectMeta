using Azure.Data.Tables;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProMe.Abstractions;
using ProMe.ApiContracts.Auth;
using ProMe.DataAccess;
using ProMe.DataAccess.Models;
using ProMe.Workflow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Commands;
public sealed record Register(RegisterRequestModel Model) : IRequest<IResult>;

public sealed class RegisterValidator : AbstractValidator<Register>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Model.Email).EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
        RuleFor(x => x.Model.OriginalPassword).Length(6, 32);
        RuleFor(x => x.Model.RepeatPassword).Equal(x => x.Model.OriginalPassword);
    }
}

internal sealed class RegisterHandler : IRequestHandler<Register, IResult>
{
    private readonly ProMeDBContext _proMeDB;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmailService _emailService;
    private readonly TableClient _emailVerificationTableClient;

    public RegisterHandler(ProMeDBContext proMeDB, IAuthenticationService authenticationService, IEmailService emailService, IConfiguration configuration)
    {
        _proMeDB = proMeDB;
        _authenticationService = authenticationService;
        _emailService = emailService;
        _emailVerificationTableClient = new TableClient(configuration.GetConnectionString("MailVerificationStorage")!, "MailVerificationKeys");
    }

    public async Task<IResult> Handle(Register request, CancellationToken cancellationToken)
    {
        try
        {
            (var key, var salt) = await _authenticationService.HashPassword(request.Model.OriginalPassword);

            var user = new User
            {
                Email = request.Model.Email,
                Key = key,
                Salt = salt,
            };

            user = (await _proMeDB.Users.AddAsync(user, cancellationToken)).Entity;

            await _proMeDB.SaveChangesAsync(cancellationToken);

            var verificationId = await _emailService.SendVerificationEmail(user.Email);

            await _emailVerificationTableClient.AddEntityAsync(new MailVerificationTableEntity
            {
                Expiration = DateTimeOffset.UtcNow.AddHours(1),
                PartitionKey = user.Email,
                RowKey = verificationId.ToString()
            });

            return Results.Created($"/Profile/{(ShortGuid)user.Id}", null);
        }
        catch (DbUpdateException)
        {
            return Results.BadRequest("An error occured while registering your account. If you already have an account, please follow the reset password option. Otherwise retry in a few minutes");
        }
    }
}