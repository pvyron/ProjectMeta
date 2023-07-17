using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProMe.Abstractions;
using ProMe.ApiContracts.Contacts;
using ProMe.DataAccess;
using ProMe.DataAccess.Models;
using ProMe.Workflow.Models;

namespace ProMe.Workflow.Commands;
public sealed record AddContact(ContactRequestModel Model) : IRequest<IResult>;

public sealed class AddContactValidator : AbstractValidator<AddContact>
{
    public AddContactValidator()
    {
        RuleFor(x => x.Model.Name).NotEmpty();
        RuleFor(x => x.Model.Email).EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
        RuleFor(x => x.Model.PhoneNumber).MaximumLength(25);
    }
}

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
        if (_identityProvider.UserId is null)
        {
            return Results.Unauthorized();
        }

        var contact = new Contact
        {
            Name = request.Model.Name!,
            Email = request.Model.Email,
            PhoneNumber = request.Model.PhoneNumber,
            UserId = _identityProvider.UserId.GetValueOrDefault()
        };

        contact = (await _proMeDB.Contacts.AddAsync(contact, cancellationToken)).Entity;

        await _proMeDB.SaveChangesAsync(cancellationToken);

        var contactData = await _proMeDB
                .Contacts
                .Include(c => c.User)
                .AsNoTracking()
                .FirstAsync(c => c.Id == contact.Id, cancellationToken)
                .Select(c => new { c.Id, c.Email, c.Name, OwnerName = c.User!.Email, c.PhoneNumber, c.UserId });

        return Results.Created($"/contacts/{contact.Id}", new ContactResponseModel
        {
            Id = contactData.Id,
            Email = contactData.Email,
            Name = contactData.Name,
            OwnerId = contactData.UserId,
            OwnerName = contactData.OwnerName,
            PhoneNumber = contactData.PhoneNumber
        });
    }
}
