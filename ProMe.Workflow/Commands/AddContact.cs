﻿using MediatR;
using Microsoft.AspNetCore.Http;
using ProMe.Abstractions;
using ProMe.DataAccess;
using ProMe.DataAccess.Models;

namespace ProMe.Workflow.Commands;
public sealed record AddContact(string Name) : IRequest<IResult>;

internal sealed class AddContactHandler : IRequestHandler<AddContact, IResult>
{
    private readonly ProMeDBContext _proMeDB;
    private readonly IIdentityProvider _identityProvider;

    public AddContactHandler(ProMeDBContext proMeDB, IIdentityProvider identityProvider)
    {
        _proMeDB = proMeDB;
        _identityProvider = identityProvider;
    }

    public async Task<IResult> Handle(AddContact request, CancellationToken cancellationToken)
    {
        var contact = new Contact
        {
            Name = request.Name,
        };

        //contact = (await _proMeDB.Contacts.AddAsync(contact)).Entity;

        //await _proMeDB.SaveChangesAsync(cancellationToken);

        //var contactGrain = _grainFactory.GetGrain<IContactGrain>(contact.Id);

        //await contactGrain.YourNameIs(request.Name);

        //return Results.Ok(new { name = await contactGrain.SayYourName() });
        return Results.Ok(new { name = request.Name, user = _identityProvider.UserId });
    }
}
