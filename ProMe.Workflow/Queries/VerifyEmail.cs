using Azure.Data.Tables;
using LanguageExt.Pretty;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProMe.DataAccess;
using ProMe.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Queries;
public sealed record VerifyEmail(string VerificationSoup) : IRequest<IResult>;

public sealed class VerifyEmailHandler : IRequestHandler<VerifyEmail, IResult>
{
    private readonly TableClient _emailVerificationTableClient;
    private readonly ProMeDBContext _proMeDB;

    public VerifyEmailHandler(IConfiguration configuration, ProMeDBContext proMeDB)
    {
        _emailVerificationTableClient = new TableClient(configuration.GetConnectionString("MailVerificationStorage")!, "MailVerificationKeys");
        _proMeDB = proMeDB;
    }

    public async Task<IResult> Handle(VerifyEmail request, CancellationToken cancellationToken)
    {
        var soup = request.VerificationSoup.Split('.');

        if (soup.Length != 2)
            return Results.NotFound();

        var part1Bytes = Convert.FromBase64String(soup[0]);
        var part2Bytes = Convert.FromBase64String(soup[1]);

        try
        {
            var key = new Guid(part1Bytes);
            var email = Encoding.ASCII.GetString(part2Bytes);

            var tableEntity = await _emailVerificationTableClient.GetEntityAsync<MailVerificationTableEntity>(email, key.ToString());

            if (tableEntity.Value is null || tableEntity.Value.Expiration < DateTime.UtcNow)
                return Results.BadRequest("Your key may be expired");

            var user = await _proMeDB.Users.FirstAsync(u => u.Email == email, cancellationToken);

            if (user is null)
                return Results.BadRequest("Your key may be expired, please register again");

            user.Verified = true;

            await _proMeDB.SaveChangesAsync(cancellationToken);

            return Results.Ok();
        }
        catch
        {
            return Results.NotFound();
        }
    }
}
