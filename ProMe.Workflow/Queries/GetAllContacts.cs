using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProMe.Abstractions;
using ProMe.ApiContracts.Contacts;
using ProMe.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Queries;
public sealed record GetAllContacts : IRequest<IResult>;

public sealed class GetAllContactsHandler : IRequestHandler<GetAllContacts, IResult>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly ProMeDBContext _proMeDB;

    public GetAllContactsHandler(IIdentityProvider identityProvider, ProMeDBContext proMeDB)
    {
        _identityProvider = identityProvider;
        _proMeDB = proMeDB;
    }

    public async Task<IResult> Handle(GetAllContacts request, CancellationToken cancellationToken)
    {
        try
        {
            if (_identityProvider.UserId is null)
                return Results.Unauthorized();

            var contacts = await _proMeDB
                .Contacts
                .Include(c => c.User)
                .AsNoTracking()
                .Where(c => c.UserId == _identityProvider.UserId)
                .Select(c => new { c.Id, c.Email, c.Name, OwnerName = c.User!.Email, c.PhoneNumber, c.UserId }) // this line could be obfuscated, however for the sake of consistancy i'll leave it here
                .Select(c => new ContactResponseModel
                {
                    Id = c.Id,
                    Email = c.Email,
                    Name = c.Name,
                    OwnerId = c.UserId,
                    OwnerName = c.OwnerName,
                    PhoneNumber = c.PhoneNumber
                })
                .ToListAsync(cancellationToken);

            if (!contacts.Any())
                return Results.NotFound();

            return Results.Ok(contacts);
        }
        catch
        {
            return Results.NotFound();
        }
    }
}
