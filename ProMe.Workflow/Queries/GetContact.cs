using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProMe.Abstractions;
using ProMe.ApiContracts.Contacts;
using ProMe.DataAccess;
using ProMe.Workflow.Commands;
using ProMe.Workflow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Queries;
public sealed record GetContact(Guid ContactId) : IRequest<IResult>;

internal sealed class GetContactHandler : IRequestHandler<GetContact, IResult>
{
    private readonly ProMeDBContext _proMeDB;
    private readonly IIdentityProvider _identityProvider;

    public GetContactHandler(ProMeDBContext proMeDB, IIdentityProvider identityProvider)
    {
        _proMeDB = proMeDB;
        _identityProvider = identityProvider;
    }

    public async Task<IResult> Handle(GetContact request, CancellationToken cancellationToken)
    {
        try
        {
            var contactData = await _proMeDB
                .Contacts
                .Include(c => c.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ContactId, cancellationToken)
                .Select(c => new { c.Id, c.Email, c.Name, OwnerName = c.User!.Email, c.PhoneNumber, c.UserId });

            if (contactData is null || !contactData.UserId.Equals(_identityProvider.UserId))
            {
                return Results.NotFound(new { Id = request.ContactId });
            }

            return Results.Ok(new ContactResponseModel
            {
                Id = contactData.Id,
                Email = contactData.Email,
                Name = contactData.Name,
                OwnerId = contactData.UserId,
                OwnerName = contactData.OwnerName,
                PhoneNumber = contactData.PhoneNumber
            });
        }
        catch
        {
            return Results.NotFound(new { Id = request.ContactId });
        }
    }
}