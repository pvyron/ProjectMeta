using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProMe.Abstractions;
using ProMe.ApiContracts.Contacts;
using ProMe.DataAccess;
using ProMe.Workflow.Models;
using ProMe.Workflow.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Commands;
public sealed record UpdateContact(Guid ContactId, ContactRequestModel Model) : IRequest<IResult>;

public sealed class UpdateContactValidator : AbstractValidator<UpdateContact>
{
    public UpdateContactValidator()
    {
        RuleFor(x => x.Model.Name).NotEmpty();
        RuleFor(x => x.Model.Email).EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
        RuleFor(x => x.Model.PhoneNumber).MaximumLength(25);
    }
}

internal sealed class UpdateContactHandler : IRequestHandler<UpdateContact, IResult>
{
    private readonly ProMeDBContext _proMeDB;
    private readonly IIdentityProvider _identityProvider;
    private readonly IMediator _mediator;

    public UpdateContactHandler(ProMeDBContext proMeDB, IIdentityProvider identityProvider, IMediator mediator)
    {
        _proMeDB = proMeDB;
        _identityProvider = identityProvider;
        _mediator = mediator;
    }

    public async Task<IResult> Handle(UpdateContact request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _proMeDB.Contacts.FindAsync(request.ContactId, cancellationToken);

            if (contact is null || !contact.UserId.Equals(_identityProvider.UserId))
            {
                return Results.NotFound(new { Id = request.ContactId });
            }

            contact.Name = request.Model.Name!;
            contact.Email = request.Model.Email;
            contact.PhoneNumber = request.Model.PhoneNumber;

            _proMeDB.Contacts.Update(contact);
            await _proMeDB.SaveChangesAsync(cancellationToken);

            var contactData = await _proMeDB
                .Contacts
                .Include(c => c.User)
                .AsNoTracking()
                .FirstAsync(c => c.Id == request.ContactId, cancellationToken)
                .Select(c => new { c.Id, c.Email, c.Name, OwnerName = c.User!.Email, c.PhoneNumber, c.UserId });

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